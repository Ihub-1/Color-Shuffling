using UdonSharp;
using UnityEngine;
using VRC.Udon.Common.Interfaces;
using VRC.SDKBase;

public class PlatformManager : UdonSharpBehaviour
{
    [SerializeField] private GameObject platformContainer;
    [SerializeField] private Material[] colorMaterials;

    private MeshRenderer[] _platforms;
    [UdonSynced] private int[] _dataPlatformsColors;
    [UdonSynced] private bool[] _dataPlatformInactive;
    [UdonSynced] public int CurrentColorIndex = 0;

    private void Start()
    {
        _platforms = GetComponentsInChildren<MeshRenderer>();
        _dataPlatformInactive = new bool[_platforms.Length];
        _dataPlatformsColors = new int[_platforms.Length];
        
        ResetToInitialState();
    }

    public void ResetToInitialState()
    {
        if (!Networking.IsOwner(Networking.LocalPlayer, gameObject)) return;
        
        for (var i = 0; i < _platforms.Length; i++)
        {
            _dataPlatformInactive[i] = false;
            _dataPlatformsColors[i] = 0;
        }
        
        OnFloorStateUpdate();
    }

    public void RandomizeFloorColor()
    {
        if (!Networking.IsOwner(Networking.LocalPlayer, gameObject)) return;
        
        for (var i = 0; i < _dataPlatformsColors.Length; i++)
        {
            _dataPlatformsColors[i] = Random.Range(0, colorMaterials.Length);
        }
        
        OnFloorStateUpdate();
    }

    public void HideTilesByColor()
    {
        if (!Networking.IsOwner(Networking.LocalPlayer, gameObject)) return;
        
        for (var i = 0; i < _platforms.Length; i++)
        {
            _platforms[i].gameObject.SetActive(_dataPlatformsColors[i] != CurrentColorIndex);
        }
        
        OnFloorStateUpdate();
    }

    public override void OnDeserialization()
    {
        OnFloorStateUpdate();
    }

    private void OnFloorStateUpdate()
    {
        for (var i = 0; i < _platforms.Length; i++)
        {
            var platform = _platforms[i];
            var color = colorMaterials[_dataPlatformsColors[i]];
            var inactive = _dataPlatformInactive[i];
            
            platform.material = color;
            platform.gameObject.SetActive(!inactive);
        }
    }
}
