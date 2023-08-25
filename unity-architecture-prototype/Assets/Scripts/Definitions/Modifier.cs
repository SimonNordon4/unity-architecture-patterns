using System;

[Serializable]
public class Modifier
{
    public StatType statType;
    public ModifierType modifierType = ModifierType.Flat;
    public float modifierValue;
}