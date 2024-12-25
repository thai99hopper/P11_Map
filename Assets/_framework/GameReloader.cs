
using UnityEngine.SceneManagement;

public class GameReloader : SingletonMonoBehaviour<GameReloader>
{
	public string loadScene;
	public void Reload()
	{
		Destroy(gameObject);
		SceneManager.LoadScene(loadScene);
	}
}