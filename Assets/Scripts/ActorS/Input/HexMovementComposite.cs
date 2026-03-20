// Assets/Scripts/Input/HexMovementComposite.cs
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Utilities;



#if UNITY_EDITOR
using UnityEditor;
#endif

#if UNITY_EDITOR
[InitializeOnLoad]
#endif
[DisplayStringFormat("{n}/{ne}/{se}/{s}/{sw}/{nw}")]
public class HexMovementComposite : InputBindingComposite<Vector2>
{
[InputControl(layout = "Button")] public int n;
[InputControl(layout = "Button")] public int ne; 
[InputControl(layout = "Button")] public int se; 
[InputControl(layout = "Button")] public int s;
[InputControl(layout = "Button")] public int sw;
[InputControl(layout = "Button")] public int nw;

    // Flat-top hex direction vectors
private static readonly Vector2 dirN  = new Vector2( 0f,    1f);
private static readonly Vector2 dirS  = new Vector2( 0f,   -1f);
private static readonly Vector2 dirNE = new Vector2(0.866f,  0.5f); 
private static readonly Vector2 dirSE = new Vector2(0.866f, -0.5f);  
private static readonly Vector2 dirSW = new Vector2(-0.866f, -0.5f);  
private static readonly Vector2 dirNW = new Vector2(-0.866f,  0.5f);  

    #if UNITY_EDITOR
    static HexMovementComposite()
    {
        InputSystem.RegisterBindingComposite<HexMovementComposite>("HexMovement");
    }
    #endif

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void Init()
    {
        InputSystem.RegisterBindingComposite<HexMovementComposite>("HexMovement");
    }

    public override Vector2 ReadValue(ref InputBindingCompositeContext context)
    {
        Vector2 result = Vector2.zero;

        if (context.ReadValueAsButton(n))  result += dirN;
        if (context.ReadValueAsButton(ne)) result += dirNE;
        if (context.ReadValueAsButton(se)) result += dirSE;
        if (context.ReadValueAsButton(s))  result += dirS;
        if (context.ReadValueAsButton(sw)) result += dirSW;
        if (context.ReadValueAsButton(nw)) result += dirNW;

        return result.magnitude > 0 ? result.normalized : Vector2.zero;
    }
}