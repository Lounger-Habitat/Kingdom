public enum ItemType
{
    Seed,Commodity,Furniture,
    HoeTool,ChopTool,BreakTool,ReapTool,WaterTool,CollectTool,
    ReapableScenery
    //种子，商品,家具，
    //锄头，斧头 ，凿石头，割草，浇水，收割，
    //可以被割的杂草
}

public enum SlotType
{
    Bag,Box,Shop,Crop
}

public enum InventoryLocation
{
    Player,Box
}

public enum PartType
{
    None,Carry,Hoe,Break,Water,Chop,Collect,Reap
}

public enum PartName
{
    Body,Hair,Arm,Tool
}

public enum Season
{
    春天,夏天,秋天,冬天
}

public enum GridType
{
    Diggable,DropItem,PlaceFurniture,NPCObstacle
}

public enum ParticaleEffectType
{
    None,LeavesFalling01,LeavesFalling02,Rock,ReapableScenery//叶子，石头，割稻草
}

public enum GameState
{
    GamePlay,Pause
}

public enum LightShift
{
    Morning,Night
}

public enum SoundName
{
    none,FootStepSoft,FootStepHard,
    Axe,Pickaxe,Hoe,Reap,Water,Basket,Chop,
    Pickup,Plant,TreeFalling,Rustle,
    AmbientCountryside1, AmbientCountryside2,MusicCalm1, MusicCalm2, MusicCalm3, MusicCalm4, MusicCalm5, MusicCalm6,AmbientIndoor1
}