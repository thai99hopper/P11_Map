
using UnityEngine;

public class CameraComponent_pan: BaseCameraComponent
{
	private CameraPanConfig config;
	private Transform cameraTransform;
	private Vector3 cacheCamPos;
	private Vector3 cacheMousePos;
	private Vector3 initialCamPos;
	private Rect? viewport;

	public Vector3 CameraPosition
	{
		get => cameraTransform.position;
		set
		{
			cameraTransform.position = value;
		}
	}

	public CameraComponent_pan(CameraPanConfig config)
	{
		this.config = config;
		cameraTransform = CameraController.instance.mainCamera.transform;
		initialCamPos = cameraTransform.position;
	}

	public override void Update()
	{
		base.Update();

		if (StaticUtils.IsBeginTouchScreen(config.panWithMouse))
		{
			cacheCamPos = CameraPosition;
			cacheMousePos = StaticUtils.GetTouchPosition();
		}

		if (StaticUtils.IsTouchingScreen(config.panWithMouse))
		{
			var dtMouse = StaticUtils.GetTouchPosition() - cacheMousePos;
			var dtCamera = DtMouseToDtCamera(dtMouse);
            CameraPosition = LimitCameraPos(cacheCamPos + dtCamera);
		}
	}

	private Vector3 LimitCameraPos(Vector3 pos)
	{
		if (viewport == null)
		{
			return pos;
		}

		var halfCameraHeight = CameraController.instance.mainCamera.orthographicSize;
		var halfCameraWidth = halfCameraHeight * ((float)Screen.width / Screen.height);

		var minX = viewport.Value.x - viewport.Value.width / 2 + halfCameraWidth;
		var maxX = viewport.Value.x + viewport.Value.width / 2 - halfCameraWidth;
		var minY = viewport.Value.y - viewport.Value.height / 2 + halfCameraHeight;
		var maxY = viewport.Value.y + viewport.Value.height / 2 - halfCameraHeight;

		pos.x = Mathf.Clamp(pos.x, minX, maxX);
		pos.y = Mathf.Clamp(pos.y, minY, maxY);

		return pos;
	}

	public void ResetPan()
	{
		CameraPosition = initialCamPos;
	}

    public void SetLimit(Rect viewport)
	{
		this.viewport = viewport;

		//set pos
		var pos = CameraPosition;
		pos.x = viewport.x;
		pos.y = viewport.y;
        CameraPosition = pos;

		//set size
		var halfViewportWidth = viewport.width / 2;
		var halfViewportHeight = viewport.height / 2;
		var cameraHeight = CameraController.instance.mainCamera.orthographicSize;
		var cameraWidth = cameraHeight * ((float)Screen.width / Screen.height);
		var scale = Mathf.Min(halfViewportWidth / cameraWidth, halfViewportHeight / cameraHeight);
		var newSize = cameraHeight * scale;
        CameraController.instance.mainCamera.orthographicSize = newSize;
	}

	private Vector3 DtMouseToDtCamera(Vector3 dtMouse)
	{
		var cameraSz = CameraController.instance.mainCamera.orthographicSize;
		var halfScreen = Screen.height / 2f;
		return -(cameraSz / halfScreen) * dtMouse;
	}
}