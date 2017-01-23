/// Modifiers that affect turret properties
/// this is mostly for special tower upgrades but might also
/// have a use for certain types of enemy having and affect on
/// nearby turrets

[System.Serializable]
public class TowerModifier {

    public Tower tower;
    public float damage;
    public float firingRange;
    public float rateOfTurn;
    public float fieldOfViewAngle;

}
