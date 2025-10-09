
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class McEditorPreview : MonoBehaviour
{
    public string AreaName => this.transform.parent.gameObject.name;

    public List<McOnMapPresenter> GetMcPresenterList()
    {
        var list = this.transform.GetComponentsInChildren<McOnMapPresenter>(true);
        return new List<McOnMapPresenter>(list);
    }
}