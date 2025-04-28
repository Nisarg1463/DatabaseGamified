using System;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuController : MonoBehaviour
{
    public GameMaster gameMaster;
    private Button execute;
    private Button roads;
    private Button exit;
    private TextField prompt;
    private TextField question;
    private Label hint;

    void OnEnable()
    {
        var root = GetComponent<UIDocument>().rootVisualElement;

        // Get menu options
        execute = root.Q<Button>("Execute");
        roads = root.Q<Button>("Roads");
        exit = root.Q<Button>("Exit");
        prompt = root.Q<TextField>("prompt-bar");
        question = root.Q<TextField>("question");
        hint = root.Q<Label>("response");

        Debug.Assert(execute != null, "Option 1 not found!");
        Debug.Assert(roads != null, "Option 2 not found!");
        Debug.Assert(exit != null, "Option 3 not found!");
        Debug.Assert(prompt != null, "Prompt not found!");
        Debug.Assert(question != null, "question not found!");
        Debug.Assert(hint != null, "hint not found!");

        // Register click events for options
        execute.clicked += () => { gameMaster.Execute(prompt.value, hint, question); };
        prompt.RegisterCallback<KeyDownEvent>(OnKeyDown, TrickleDown.TrickleDown);
        roads.clicked += () => gameMaster.ToggleRoad();
        exit.clicked += () => Application.Quit();
    }
    private void OnKeyDown(KeyDownEvent evt)
    {
        // Check if the Enter key was pressed
        if (evt.keyCode == KeyCode.Return || evt.keyCode == KeyCode.KeypadEnter)
        {
            Debug.Log("Submitted: " + prompt.value);

            gameMaster.Execute(prompt.value, hint, question);

            // Optionally stop propagation to prevent other handlers from firing
            evt.StopImmediatePropagation();
        }
    }
}
