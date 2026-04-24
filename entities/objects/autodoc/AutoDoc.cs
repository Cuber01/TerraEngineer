using Godot;
using System;
using TENamespace.player_inventory;
using TENamespace.save_entity;
using TENamespace.ui.dialogue_box;
using TerraEngineer;
using TerraEngineer.entities.mobs;
using TerraEngineer.game;

public partial class AutoDoc : Entity
{
	[Export] private StringName itemName;
	[Export] private StringName itemCollectedTag;
	[Export] private Resource dialogueDescription;
	
	private DialogueBalloon balloonTemplate;
	private Player player;
	private bool closed = false;

	#region Init
	public override void _Ready()
	{
		SpriteWrapper.Init(Sprite);
		CM.GetComponent<SaveEntity>().Setup(itemCollectedTag, close);
		GlobalEventBus.Instance.Subscribe(GlobalEvents.BossEntered, close);
		GlobalEventBus.Instance.Subscribe(GlobalEvents.BossDefeated, open);
		CM.GetComponent<SaveEntity>().OptionalInit(this);
		
		
		player = GetNode<Player>(Names.NodePaths.Player);
		balloonTemplate = GetNode<DialogueBalloon>(Names.NodePaths.DialogueBalloon);

	}

	private void close(Node node)
	{
		closed = true;
		SpriteWrapper.Play("closed");
	}
	
	private void close()
	{
		closed = true;
		SpriteWrapper.Play("closed");
	}

	private void open()
	{
		closed = false;
		SpriteWrapper.Play("open");
	}
	#endregion
	
	private void onPlayerEntered(Player player)
	{
		if(closed) return;
		
		player.Controller.AddOverride(Names.Actions.Attack, player.InvokeInteracted);
		player.Interacted += enter;
	}
    
	private void onPlayerExited(Player player)
	{
		player.Controller.RemoveOverride(Names.Actions.Attack);
		player.Interacted -= enter;
	}

	private void enter()
	{
		Action whileInside = null;
		whileInside = () =>
		{
			balloonTemplate.PlayDialogue(dialogueDescription, Names.Other.Start);
			SpriteWrapper.AnimationFinished -= whileInside; 
		};
		
		// Enter
		SpriteWrapper.AnimationFinished += whileInside;
		SpriteWrapper.Play("closing");
		player.Hide();
		player.Freeze();
		player.Controller.SwitchControl(balloonTemplate.Controller);
		
		Action leave = null;
		leave = () =>
		{
			player.Show();
			player.Unfreeze();
			CM.GetComponent<SaveEntity>().ChangeState(true);
			player.CM.GetComponent<PlayerInventory>().AddUniqueItem(player, itemName);
		
			SpriteWrapper.Play("closing");
			closed = true;
			SpriteWrapper.AnimationFinished -= leave;
		};
		
		TimerManager.Schedule(3f, this, (_) =>
		{
			SpriteWrapper.Play("closing", -1);
			SpriteWrapper.AnimationFinished += leave;
		});
		

	}


}
