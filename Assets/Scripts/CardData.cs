using UnityEngine;

[CreateAssetMenu(fileName = "NewCard", menuName = "Card")]
public class CardData : ScriptableObject
{
    public string cardName;
    public int attack;
    public int health;
    public int speed;
    public ElementType type1;
    public ElementType type2;
    public PassiveAbilityType passiveAbility;
    public ElementType abilityTarget;
    public int cost;
    public string abilityDetails;
    public Sprite artwork;

   
    public bool isTerrain = false;
    public TerrainType terrainType = TerrainType.Ninguno;
    public int buffAttackAmount = 0;
    public int buffHealthAmount = 0;

    
    public CardData cartaInvocada;

    [HideInInspector]
    public bool haResucitado = false; 

}


public enum ElementType
{
    Gigante, Hada, NoMuerto, Humano, Dragon, Bestia, Elemental, Demonio
}

public enum TerrainType
{
    Ninguno,
    Pantano,
    Prado,
    Cementerio
}

public enum PassiveAbilityType
{
    Ninguna,
    DañoExtra,
    ReduccionDeDaño,
    UsarVariosHechizos,
    DestruirTerreno,
    Provocar,
    OtorgarAtributo,
    InvocarCadaTurno,
    InvocarAlEntrar,
    LanzarDado,
    ResucitarCadaTurno
}
