# Inverse-Kinematics

After you have important the package, you may receive the error " HeadIKEditor.cs(92,21): error CS1061: 'ThirdPersonController' does not contain a definition for 'CurrentSpeed' " if thats the case, no worries, open up the unity script (or your own player script) and add the following lines:

public float CurrentSpeed()
    return _speed;

This will work perfectly and fix the error if you are using the Unity3D ThirdPerson Starter Assets, how ever if you are using your own third person controller then tweak “_speed;” to just return the current movement of your player.
