using R3;
using UnityEngine;

public partial class BuildingOnMapController : MonoBehaviour
{
    public ReactiveProperty<bool> visibleRx { get; set; } = new(true);
}