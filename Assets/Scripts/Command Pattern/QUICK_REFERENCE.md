# Command Pattern Quick Reference

## 🚀 **Quick Start**

### **Setup Required Components:**
1. **InputHandler** - Attach to GameObject with input handling
2. **MovementController** - Attach to player GameObject
3. **Assign in Inspector:**
   - MovementController reference
   - Audio clips (mainEngine, shieldActivationSound)
   - Particle systems (rocketBoost, leftThrust, rightThrust)

### **Input Mapping:**
- **Thrust**: Space bar or "Thrust" button
- **Rotate Left**: A key or "Left" button
- **Rotate Right**: D key or "Right" button
- **Shield**: E key or "Shield" button

## 📁 **File Structure**

```
Command Pattern/
├── InputHandler.cs          # Main controller
├── MovementController.cs    # Component provider
├── Invoker.cs              # Command executor
├── Command.cs              # Base command interface
├── MoveUp.cs               # Thrust command
├── StopThrust.cs           # Stop thrust command
├── RotateLeft.cs           # Left rotation command
├── RotateRight.cs          # Right rotation command
├── StopRotation.cs         # Stop rotation command
├── ToggleShield.cs         # Shield toggle command
├── README.md               # Full documentation
└── QUICK_REFERENCE.md      # This file
```

## ⚡ **Performance Tips**

### **Critical Optimizations:**
- ✅ **Component Caching**: All components cached in Awake()
- ✅ **Early Returns**: Prevents unnecessary Update() processing
- ✅ **Null Safety**: Comprehensive null checks throughout
- ✅ **Readonly Fields**: Immutable command parameters

### **Memory Management:**
- **No GC Pressure**: Minimal allocations in Update loops
- **Efficient Audio**: Uses PlayOneShot() for non-looping sounds
- **Particle Management**: Proper Play()/Stop() calls

## 🔧 **Common Tasks**

### **Add New Command:**
```csharp
// 1. Create new command class
public class NewCommand : Command
{
    public override void Execute(Rigidbody rigidbody, AudioSource audioSource)
    {
        // Your logic here
    }
}

// 2. Add to InputHandler.InitializeCommands()
newCommand = new NewCommand(parameters);

// 3. Add input handling in Handle*Input() method
```

### **Modify Input Mapping:**
```csharp
// In InputHandler.HandleThrustInput()
bool isThrusting = Input.GetKey(KeyCode.Space) || 
                   CrossPlatformInputManager.GetButton("Thrust");
```

### **Add New Component Reference:**
```csharp
// 1. Add field
[SerializeField] private NewComponent newComponent;

// 2. Cache in Awake()
newComponent = FindObjectOfType<NewComponent>();

// 3. Pass to commands in InitializeCommands()
```

## 🐛 **Troubleshooting**

### **No Movement:**
- Check Rigidbody component exists on MovementController
- Verify InputHandler has MovementController assigned
- Check console for null reference errors

### **No Audio:**
- Verify AudioSource component on MovementController
- Check audio clips assigned in InputHandler inspector
- Ensure audio files are properly imported

### **No Particles:**
- Verify particle systems assigned in InputHandler inspector
- Check particle systems are not disabled
- Ensure particle systems have proper materials

### **Input Not Working:**
- Check InputHandler is enabled
- Verify game state (not in tutorial/level over)
- Check console for component validation errors

## 📊 **Performance Monitoring**

### **Key Metrics to Watch:**
- **Update() Calls**: Should be minimal processing
- **Component Searches**: Should only happen in Awake()
- **Memory Allocations**: Should be minimal in Update()
- **Audio Playback**: Should use PlayOneShot() efficiently

### **Debug Logs:**
- **Component Validation**: Automatic null checks
- **Command Execution**: Error logging in Invoker
- **State Changes**: Game state transitions logged

## 🎯 **Best Practices**

### **Do's:**
- ✅ Cache components in Awake()/Start()
- ✅ Use early returns in Update()
- ✅ Add null checks for all components
- ✅ Use readonly for immutable fields
- ✅ Handle errors with try-catch

### **Don'ts:**
- ❌ Call FindObjectOfType() in Update()
- ❌ Allocate objects in Update()
- ❌ Skip null checks
- ❌ Use public fields (use [SerializeField] instead)
- ❌ Ignore error handling

## 🔄 **Architecture Flow**

```
Input → InputHandler → Invoker → Command → Unity Components
  ↓         ↓           ↓         ↓           ↓
Keyboard  Process    Execute   Apply      Rigidbody
Buttons    Input      Command   Logic      AudioSource
```

This Command Pattern system provides a clean, performant, and maintainable input handling solution! 🚀 