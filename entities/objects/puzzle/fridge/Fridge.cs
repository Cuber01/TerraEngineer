using DialogueManagerRuntime;
using Godot;
using TENamespace.player_inventory;
using TENamespace.save_entity;
using TENamespace.ui.dialogue_box;
using TerraEngineer;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.objects;

public partial class Fridge : Entity, IInteractable
{
    [Export] private Resource dialogue;
    
    private DialogueBalloon balloonTemplate;
    private Player player;
    public bool InteractionBlocked { get; set; }
    
    private bool hasCrystalInFridge = false;

    public override void _Ready()
    {
        SpriteWrapper.Init(Sprite);
        
        CM.GetComponent<SaveEntity>().Setup("fridge_has_crystal", (_) =>
        {
            hasCrystalInFridge = true;
        });
        
        CM.GetComponent<SaveEntity>().OptionalInit(this);
        
        player = GetNode<Player>(Names.NodePaths.Player);
        balloonTemplate = GetNode<DialogueBalloon>(Names.NodePaths.DialogueBalloon);
    }

    public void OnInteracted()
    {
        bool playerHasCrystal = player.CM.GetComponent<PlayerInventory>().HasItem("ice_crystal");
        
        StringName dialogueTitle;
        
        if (hasCrystalInFridge)
        {
            dialogueTitle = "crystal_in_fridge";
        }
        else if (playerHasCrystal)
        {
            dialogueTitle = "has_crystal";
        }
        else
        {
            dialogueTitle = "start";
        }
        
        balloonTemplate.PlayDialogue(dialogue, dialogueTitle);
        player.Controller.SwitchControl(balloonTemplate.Controller);
        
        DialogueManager.DialogueEnded += onDialogueEnded;
    }
    
    private void onDialogueEnded(Resource dialogueResource)
    {
        DialogueManager.DialogueEnded -= onDialogueEnded;
        
        int choice = GlobalDialoguesState.Instance.PuzzleFridge_Choice;
        GlobalDialoguesState.Instance.PuzzleFridge_Choice = 0;
        
        if (choice == 1)
        {
            // Player chose Yes
            Variant hasCrystal = SaveData.ReadValue(Names.SaveSections.PlayerInventory, "ice_crystal");
            bool playerHasCrystal = MathT.IsTrue(hasCrystal);
            
            if (playerHasCrystal && !hasCrystalInFridge)
            {
                // Put crystal in fridge
                SaveData.SetAddValue(Names.SaveSections.PlayerInventory, "ice_crystal", false);
                hasCrystalInFridge = true;
                CM.GetComponent<SaveEntity>().ChangeState(true);
            }
            else if (hasCrystalInFridge && !playerHasCrystal)
            {
                // Take crystal from fridge
                player.CM.GetComponent<PlayerInventory>().AddUniqueItem("ice_crystal");
                hasCrystalInFridge = false;
                CM.GetComponent<SaveEntity>().ChangeState(false);
            }
        }
        // choice == 0 - do nothing
    }
}
