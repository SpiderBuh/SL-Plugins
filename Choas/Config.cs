using RedRightHand.CustomPlugin;

namespace Choas;

public class Config : CustomPluginConfig
{
    // Enable/disable all custom keybinds
    public bool EnableCustomKeybinds { get; set; } = false;
    
    // Old features
    public bool EnableOldMechanics { get; set; } = false;
    
    
}