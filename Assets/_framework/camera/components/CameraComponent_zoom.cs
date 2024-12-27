public class CameraComponent_zoom : BaseCameraComponent
{
	private float initialZoom;
	private CameraZoomConfig config;

	public float CameraZoom
	{
		get => CameraController.instance.mainCamera.orthographicSize;
		set
		{
			CameraController.instance.mainCamera.orthographicSize = value;
		}
	}

	public CameraComponent_zoom(CameraZoomConfig config)
	{
		this.config = config;
		initialZoom = CameraController.instance.mainCamera.orthographicSize;
	}

	public override void Update()
	{
		base.Update();

        CameraZoom -= StaticUtils.GetMouseScrollDelta() * config.scrollSpeed;
	}

	public void ResetZoom()
	{
        CameraZoom = initialZoom;
	}
}