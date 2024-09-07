using UnityEngine;

[CreateAssetMenu(fileName = "bloomie_bundle", menuName = "Bloomie Bundle")]
public class BloomeBundle_SO : ScriptableObject
{
    [SerializeField]
    private Sprite imageUI; // For UI image representation

    [SerializeField]
    private string name; // Name of the object

    [SerializeField]
    private float bonusperc; // Description of the object

    [SerializeField]
    private float amount; // Amount or quantity of the object

    // Getters and Setters for each field, if needed, can go here.
    public Sprite ImageUI
    {
        get { return imageUI; }
        set { imageUI = value; }
    }

    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public float BonusPerc
    {
        get { return bonusperc; }
        set { bonusperc = value; }
    }

    public float Amount
    {
        get { return amount; }
        set { amount = value; }
    }
}