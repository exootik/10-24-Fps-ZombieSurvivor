using UnityEngine;

public class FreeZombie_EyesGlow : MonoBehaviour
{
    public enum EyesGlow
    {
        No,
        Yes
    }

    public Material[] BodyMaterials = new Material[1];


    public EyesGlow eyesGlow;


    private int eyesTyp;

    private void OnValidate()
    {
        if (eyesGlow == 0)
        {
            BodyMaterials[0].DisableKeyword("_EMISSION");
            BodyMaterials[0].SetFloat("_EmissiveExposureWeight", 1);
        }
        else
        {
            BodyMaterials[0].EnableKeyword("_EMISSION");
            BodyMaterials[0].SetFloat("_EmissiveExposureWeight", 0);
        }
    }
}