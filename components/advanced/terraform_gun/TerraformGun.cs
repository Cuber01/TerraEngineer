using Godot;
using TENamespace.basic.particle_builder;
using TerraEngineer.entities.objects;

namespace TENamespace.advanced.terraform_gun;

public partial class TerraformGun : AdvancedComponent, IGun
{
    public delegate void EssenceChangedEventHandler(Biomes selected, Biomes unselected);
    public event EssenceChangedEventHandler EssenceChanged;
    
    public delegate void EssenceUnlockedEventHandler(Biomes biome);
    public event EssenceUnlockedEventHandler EssenceUnlocked;
    
    public delegate void EssenceLockedEventHandler(Biomes biome);
    public event EssenceLockedEventHandler EssenceLocked;

    public Biomes SelectedBiome
    {
        get => _selectedBiome;
        private set
        {
            if (_selectedBiome != value)
            {
                EssenceChanged?.Invoke(value, _selectedBiome);
                _selectedBiome = value;
            } 
        }
    }
    private Biomes _selectedBiome = Biomes.Forest;
    
    [Export] private Area2D areaAffected;

    public bool Usable = false;
    
    // Every enum biome corresponds to it's index in this array
    private Biomes[] modes = [Biomes.Locked, Biomes.Locked, Biomes.Locked, Biomes.Locked];

    public void Shoot(Vector2 position, Vector2 direction, float rotationDegrees)
    {
        CM.GetComponent<StarParticleSpawner>()
                  .Start()
                  .SetBiome(_selectedBiome)
                  .SetPosition(position)
                  .SetDirectionNormal(direction)
                  .Build();

        CM.GetComponent<StarParticleSpawner>().AddToGame();
        
        // Update area position and rotation
        areaAffected.GlobalPosition = position;
        areaAffected.RotationDegrees = rotationDegrees;
        
        // Defer terraform to next physics frame to ensure overlap detection is updated
        CallDeferred(MethodName.applyTerraform);
    }

    public void ChangeWeapon(int index)
    {
        if (modes[index] != Biomes.Locked)
        {
            Biomes toSelect = (Biomes)index;
            SelectedBiome = toSelect;
        }
    }

    public void ChangeToNextWeapon()
    {
        int i = (int)_selectedBiome + 1;
        while (true)
        {
            if (i == modes.Length)
            {
                // Loop back
                i = 0;
            }
            
            if (modes[i] != Biomes.Locked)
            {
                SelectedBiome = (Biomes)i;
                break;
            }
            
            i++;
        }
    }

    public void LockOrUnlockMode(Biomes biome, bool unlock)
    {
        modes[(int)biome] = unlock ? biome : Biomes.Locked;
        if (unlock)
        {
            EssenceUnlocked?.Invoke(biome);
            Usable = true;
        }
        else
        {
            EssenceLocked?.Invoke(biome);
            
            // If the locked biome was the selected one, switch gun handle back to Pistol
            if (_selectedBiome == biome)
            {
                GetParent().GetParent<GunHandle>().ChangeGunHandle();
            }
            
            // Update Usable based on whether any essences are unlocked
            Usable = false;
            foreach (var mode in modes)
            {
                if (mode != Biomes.Locked)
                {
                    Usable = true;
                    break;
                }
            }
        }
    }
    
    public bool IsModeUnlocked(Biomes biome)
    {
        return modes[(int)biome] != Biomes.Locked;
    }
    
    public Biomes[] GetModes()
    {
        return modes;
    }
    
    private void applyTerraform()
    {
        var collisionShape = areaAffected.GetNode<CollisionShape2D>("CollisionShape2D");
        var query = new PhysicsShapeQueryParameters2D();
        query.Shape = collisionShape.Shape;
        query.Transform = areaAffected.GetGlobalTransform();
        query.CollisionMask = areaAffected.CollisionMask;
        
        var spaceState = GetWorld2D().DirectSpaceState;
        var results = spaceState.IntersectShape(query);
        
        foreach (var result in results)
        {
            var collider = (Node)result["collider"];
            if (collider is TerraformableCaretaker terraformable)
            {
                terraformable.Terraform(_selectedBiome);
            }
        }
    }

}