namespace BiggerDrops.Data
{
    public class DropSlotDef {
        public DropDescriptionDef Description { get; set; } = new DropDescriptionDef();
        public bool Disabled { get; set; } = false; // slot is disabled
        public bool PlayerControl { get; set; } = true; // slot is under player control 
        public float difficultyWeight { get; set; } = 1.0f; // ? weight used by BD ?
        public string StatName { get; set; } = "BD_UnknownSlotType"; // stat used to track how many of this slot is available
        public int Order { get; set; } = 1; //  the order compared to other slot types
        public bool SeparateLance { get; set; } = false; // slot cannot be mixed with other slots in a Lance
        public bool HotDrop { get; set; } = false; // Can the Slot be used for a delayed "Hot Drop",
                                                   // Hot Drop Slots are automatically separated into a distinct group of lances
    }
}