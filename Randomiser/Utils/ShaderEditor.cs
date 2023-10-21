using UnityEngine;

namespace Randomiser.Utils;

#if DEBUG
public class ShaderEditor : MonoBehaviour
{
    private Renderer renderer;

    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    private string shader = "";

    private void OnGUI()
    {
        Colour("_Color", 0, 1);

        Float("_UberShaderColorMask", 0, 16);
        Float("_UberShaderBlendModeDst", 0, 32);
        Float("_MaskStrength", 0, 1);

        Vec4("_ScreenMask", 0, 1);

        shader = GUILayout.TextField(shader);
        if (GUILayout.Button("Set Shader"))
            renderer.material.shader = Shader.Find(shader);

        GUILayout.EndArea();
    }

    private void Float(string name, float left, float right)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(name, GUILayout.Width(200));
        renderer.material.SetFloat(name, GUILayout.HorizontalSlider(renderer.material.GetFloat(name), left, right));
        GUILayout.Label(renderer.material.GetFloat(name).ToString(), GUILayout.Width(100));
        GUILayout.EndHorizontal();
    }

    private void Vec4(string name, float min, float max)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(name, GUILayout.Width(200));
        var vec = renderer.material.GetVector(name);
        vec.x = GUILayout.HorizontalSlider(vec.x, min, max);
        vec.y = GUILayout.HorizontalSlider(vec.y, min, max);
        vec.z = GUILayout.HorizontalSlider(vec.z, min, max);
        vec.w = GUILayout.HorizontalSlider(vec.w, min, max);
        GUILayout.EndHorizontal();
        renderer.material.SetVector(name, vec);
    }

    private void Colour(string name, float min, float max)
    {
        GUILayout.BeginHorizontal();
        GUILayout.Label(name, GUILayout.Width(200));
        var col = renderer.material.GetColor(name);
        col.r = GUILayout.HorizontalSlider(col.r, min, max);
        col.g = GUILayout.HorizontalSlider(col.g, min, max);
        col.b = GUILayout.HorizontalSlider(col.b, min, max);
        col.a = GUILayout.HorizontalSlider(col.a, min, max);
        GUILayout.EndHorizontal();
        renderer.material.SetColor(name, col);
    }
}
#endif
