using Godot;
using System;
using TENamespace.player_inventory;
using TENamespace.save_entity;
using TENamespace.ui.dialogue_box;
using TerraEngineer;
using TerraEngineer.entities.mobs;
using TerraEngineer.entities.objects;
using TerraEngineer.game;
using TerraEngineer.game.ui;

public partial class AutoDoc : Entity, IInteractable
{
	[Export] private StringName itemName;
	[Export] private StringName itemCollectedTag;
	[Export] private Resource dialogueDescription;
	
	private DialogueBalloon balloonTemplate;
	private Player player;
	public bool InteractionBlocked { get; set; }

	#region Init
	public override void _Ready()
	{
		SpriteWrapper.Init(Sprite);

		CM.GetComponent<SaveEntity>().Setup(itemCollectedTag, closeInstantly);
		GlobalEventBus.Instance.Subscribe(GlobalEvents.BossEntered, closeInstantly);
		GlobalEventBus.Instance.Subscribe(GlobalEvents.BossDefeated, open);
		
		CM.GetComponent<SaveEntity>().OptionalInit(this);
		
		player = GetNode<Player>(Names.NodePaths.Player);
		balloonTemplate = GetNode<DialogueBalloon>(Names.NodePaths.DialogueBalloon);

	}
	
	private void closeInstantly()
	{
		SpriteWrapper.Play(Names.Animations.Closed);
		InteractionBlocked = true;
	}

	private void closeInstantly(Node2D _) => closeInstantly();
	
	private void open()
	{
		SpriteWrapper.Play(Names.Animations.Open);
	}

	public override void _ExitTree()
	{
		GlobalEventBus.Instance.Unsubscribe(GlobalEvents.BossEntered, closeInstantly);
		GlobalEventBus.Instance.Unsubscribe(GlobalEvents.BossDefeated, open);
	}
	
	#endregion

	public void OnInteracted()
	{
		Action whileInside = null;
		whileInside = () =>
		{
			balloonTemplate.PlayDialogue(dialogueDescription, Names.Other.Start);
			SpriteWrapper.AnimationFinished -= whileInside; 
		};
		
		// Enter
		SpriteWrapper.AnimationFinished += whileInside;
		SpriteWrapper.Play(Names.Animations.Closing);
		player.Hide();
		player.Freeze();
		InputStackManager.Push(balloonTemplate.InputContext);
		
		Action leave = null;
		leave = () =>
		{
			player.Show();
			player.Unfreeze();
			CM.GetComponent<SaveEntity>().ChangeState(true);
			player.CM.GetComponent<PlayerInventory>().AddUniqueItem(itemName);
		
			SpriteWrapper.Play(Names.Animations.Closing);
			InteractionBlocked = true;
			SpriteWrapper.AnimationFinished -= leave;
		};
		
		TimerManager.Schedule(3f, this, (_) =>
		{
			SpriteWrapper.Play(Names.Animations.Closing, -1);
			SpriteWrapper.AnimationFinished += leave;
		});
	}
}
