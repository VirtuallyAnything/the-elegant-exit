using Godot;
using Godot.Collections;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using tee;

//Code Source: https://github.com/87PizzaStudios/godot_light_fow/tree/main?tab=MIT-1-ov-file#readme
// Translated to C# and adapted for The Elegant Exit
public partial class Veil : TextureRect
{
	// The texture to overlay as fog of war
	[Export] private Texture2D _fogTexture;
	// Fog scroll velocity
	[Export] private Vector2 _fogScrollVelocity = new Vector2(0.1f, 0.1f);
	// Determines whether observed areas stay revealed after moving to a new area
	[Export] private bool _persistentReveal = true;
	// Name of the group of lights that will reveal fog of war
	[Export] private string _lightGroup;
	// Name of the group of light occluders
	[Export] private string _lightOccluderGroup;
	// Scales down the fow texture for more efficient computation
	[Export, Range(0.05, 1.0)] private float _fowScaleFactor = 1;
	// Whether to reveal at initial light location
	[Export] private bool _initialReveal = true;

	private SubViewport _lightSV;
	private SubViewport _maskSV;
	private TextureRect _mask;

	private Image _maskImage;
	private Texture2D _maskTexture;
	private Vector2I _sizeVec;

	// pairs of original and duplicate lights and occluders
	private PointLight2D _playerVision;
	private PointLight2D _playerVisionDup;
	private Dictionary<ulong, LightOccluder2D> _occluderDupsDict = new();

	public override void _EnterTree()
	{
		base._EnterTree();
		PlayerMovement.NodeMoved += OnPlayerMoved;
		DynamicLightOccluder2D.TransformChanged += UpdateOccluderTransform;
	}

	public override void _Ready()
	{
		_lightSV = GetNode<SubViewport>("LightSubViewport");
		_maskSV = GetNode<SubViewport>("MaskSubViewport");
		_mask = GetNode<TextureRect>("MaskSubViewport/TextureRect");
		var display_width = Size.X;
		var display_height = Size.Y;
		Vector2 displayVector = new Vector2(display_width, display_height);
		_sizeVec = (Vector2I)(_fowScaleFactor * displayVector);

		// set Viewports and TextureRects to window size
		_lightSV.Size = _sizeVec;
		_maskSV.Size = _sizeVec;
		GetNode<TextureRect>("LightSubViewport/Background").Size = _sizeVec;
		GetNode<TextureRect>("MaskSubViewport/TextureRect").Size = _sizeVec;

		// mask
		_maskImage = Godot.Image.CreateEmpty((int)display_width, (int)display_height, false, Image.Format.Rgba8);
		_maskImage.Fill(Color.Color8(0, 0, 0, 255));
		_maskTexture = ImageTexture.CreateFromImage(_maskImage);

		// add a copy of the light to the light subviewport
		_playerVision = (PointLight2D)GetTree().GetFirstNodeInGroup(_lightGroup);
		_playerVisionDup = (PointLight2D)_playerVision.Duplicate();
		_playerVisionDup.Color = Color.Color8(255, 255, 255, 255);
		_playerVisionDup.Energy = 1.0f;
		_playerVisionDup.ShadowEnabled = true;
		_playerVisionDup.Position = _playerVision.GlobalPosition * _fowScaleFactor;
		_playerVisionDup.ApplyScale(_fowScaleFactor * Vector2.One);
		_lightSV.AddChild(_playerVisionDup);

		// set shader params
		ShaderMaterial maskMaterial = (ShaderMaterial)_mask.Material;
		maskMaterial.SetShaderParameter("persistent_reveal", _persistentReveal);
		ShaderMaterial thisMaterial = (ShaderMaterial)Material;
		thisMaterial.SetShaderParameter("fog_scroll_velocity", _fogScrollVelocity);
		thisMaterial.SetShaderParameter("fog_texture", _fogTexture);
		thisMaterial.SetShaderParameter("mask_texture", _maskTexture);
		if (_persistentReveal)
		{
			maskMaterial.SetShaderParameter("mask_texture", _maskTexture);
		}
	}

	/// <summary>
	/// Called only after all occluders on the current floor have been initiated. 
	/// Adds copies of the light occluders in _lightOccludersGroup to the light subviewport
	/// </summary>
	public void SetupOccluders()
	{
		foreach (LightOccluder2D occluder in GetTree().GetNodesInGroup(_lightOccluderGroup).Cast<LightOccluder2D>())
		{
			if (occluder is LightOccluder2D)
			{
				if (occluder is DynamicLightOccluder2D)
				{
					occluder.SetNotifyTransform(true);
				}
				LightOccluder2D occluderDup = new()
				{
					Occluder = occluder.Occluder
				};
				occluderDup.Position = occluder.GlobalPosition * _fowScaleFactor;
				occluderDup.Rotation = occluder.GlobalRotation;
				occluderDup.Scale = occluder.GlobalScale;
				occluderDup.ApplyScale(_fowScaleFactor * Vector2.One);
				_lightSV.AddChild(occluderDup);
				_occluderDupsDict[occluder.GetInstanceId()] = occluderDup;
			}
		}

		// reveal at initial light locations
		if (_initialReveal)
		{
			// workaround for node initialization order (possible engine bug?)
			InitialReveal();
		}
	}

	/// <summary>
	/// Function to update position and rotation of Light Occluders in _lightSV if their counterparts have been transformed.
	/// </summary>
	/// <param name="occluder"></param>
	private void UpdateOccluderTransform(LightOccluder2D occluder)
	{
		ulong instanceId = occluder.GetInstanceId();
		if (_occluderDupsDict.ContainsKey(instanceId))
		{
			LightOccluder2D occluderDup = _occluderDupsDict[instanceId];
			occluderDup.Rotation = occluder.GlobalRotation;
			occluderDup.Position = occluder.GlobalPosition * _fowScaleFactor;
		}

	}

	private async void InitialReveal()
	{
		await ToSignal(GetTree(), "process_frame");
		await ToSignal(GetTree(), "physics_frame");

		OnPlayerMoved(_playerVisionDup.GlobalPosition);
	}

	/// <summary>
	/// Takes the instance ID and position of the in-game light to move the duplicate light and update fog of war
	/// </summary>
	/// <param name="position">Position the player moved to.</param>
	/// <param name="rotation">Rotation of the player.</param>
	public void OnPlayerMoved(Vector2 position, float rotation = 0)
	{
		_playerVisionDup.Position = _fowScaleFactor * position;
		_playerVisionDup.Rotation = rotation;
		_maskImage = _maskSV.GetTexture().GetImage();
		_maskTexture = ImageTexture.CreateFromImage(_maskImage);
		ShaderMaterial thisMaterial = (ShaderMaterial)Material;
		thisMaterial.SetShaderParameter("mask_texture", _maskTexture);
		if (_persistentReveal)
		{
			ShaderMaterial maskMaterial = (ShaderMaterial)_mask.Material;
			maskMaterial.SetShaderParameter("mask_texture", _maskTexture);
		}
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		PlayerMovement.NodeMoved -= OnPlayerMoved;
		DynamicLightOccluder2D.TransformChanged -= UpdateOccluderTransform;
	}
}
