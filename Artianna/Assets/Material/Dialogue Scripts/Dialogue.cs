using UnityEngine;

[System.Serializable]
public class Dialogue
{
    public DialogueLine[] lines;
}

[System.Serializable]
public class DialogueLine
{
    
    public string characterNameLeft;
    public Sprite portraitLeft;
    public Sprite portraitRight;

    [TextArea(2, 5)]
    public string text;

    public bool isChoice;
    public string goodChoiceText;
    public string badChoiceText;

    [TextArea(2, 5)]
    public string goodResultText;
    [TextArea(2, 5)]
    public string badResultText;

    public Sprite dialogueImage; // Картинка для диалога (она будет показана на всех этапах)
    public Sprite goodDialogueImage; // Картинка, которая показывается после хорошего выбора
    public Sprite badDialogueImage;  // Картинка, которая показывается после плохого выбора
    public Sprite goodPortraitLeft; // Портрет после хорошего выбора
    public Sprite badPortraitLeft;  // Портрет после плохого выбора
    public GameObject additionalObject; // Дополнительный объект, который появится после выбора
    public GameObject backgroundObject; // Фон, который может быть отображен или скрыт
    public bool useBackground; // Использовать фон для данного диалога
}