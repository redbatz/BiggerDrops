namespace BiggerDrops.Data
{
    public class DropSlotDef {
        public DropDescriptionDef Description { get; set; } = new DropDescriptionDef();
        public bool Disabled { get; set; } = false; // slot is disabled
        public bool PlayerControl { get; set; } = true; // slot is under player control 
        public float difficultyWeight { get; set; } = 1.0f; // ? weight used by BD ?
        public string StatName { get; set; } = "BD_UnknownSlotType";
    }
}