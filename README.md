## Non-Playmode Physics Simulator

Unity physical simulation activated in Unity's Play Mode is used in Edit Mode to allow natural placement of objects using rigid bodies and colliders using physical simulation. When Play Mode starts, objects are trapped or space is left, allowing for stable placement of objects by calculating unnecessary movements in advance.

![sample video](https://blog.kakaocdn.net/dn/c61wHT/btst6O5rjwH/vOC2wWd15CK37zA8zsQnD0/img.gif)

The **Non-Playmode Physics Simulator** is a powerful Unity Editor tool that allows you to interactively simulate and observe physics behavior within the Unity Editor. It focuses on providing a controlled environment for simulating physics interactions without entering Play mode. Please note that this tool primarily targets simulation capabilities and does not handle unpredictable or exceptional cases. Make sure to understand how to use it and its precautions before proceeding.

### Usage

1. Open the **Non-Playmode Physics Simulator** window from the "Tools" menu in the Unity Editor.
   
2. In the window, you will find two main buttons:
   - **Enable Physics**: Click this button to activate physics simulation. This will start the simulation.
   - **Disable Physics**: Click this button to deactivate physics simulation. This will pause the simulation.

3. While physics simulation is active:
   - You can interact with GameObjects in your Scene as usual, including modifying their positions, rotations, or scales.
   - Observe how real-time physics simulation responds to these changes.

4. To pause or stop the simulation, click the "Disable Physics" button.

5. You can simulate Rigidbody components on selected GameObjects by clicking the "Lock Except Selections" button. This freezes unselected object's physics properties temporarily.

6. After the simulation, use the "Unlock All" button to release the locks applied to Rigidbody components. It is crucial to unlock them to restore normal physics behavior after the simulation.

7. Holding down the "Enable Physics" button allows you to continuously update the simulation mode.


##

- Select **Tools/Bonnate/Non-Playmode Physics Simulator**.

![Non-Playmode Physics Simulator](https://blog.kakaocdn.net/dn/2KRBy/btsubMsr4Zj/vkLHX3csPr0vEcdVn2Wkxk/img.png)

- Click the green button labeled **Enable Physics** to activate physics simulation.
- Conversely, click the red button labeled **Disable Physics** to deactivate it.

![Enable and Disable Physics](https://blog.kakaocdn.net/dn/7BIzi/btsut1H8llm/Y7zaYcaW8uqk1unB9inxK1/img.png)

- [Caution] Clicking **Lock Except Selections** will set the Constraints to FreezeAll for all active Rigidbody components that are not currently selected. This setting can be reverted to its original state by clicking the **Unlock All** button.
- [Caution] However, if you save the Scene or perform unpredictable actions like closing Unity Editor while the Lock is active, the applied settings might persist. If you have applied Lock, be sure to click **Unlock All** to return to the original state.

##

### Notes and Precautions

- This tool is designed for simulating physics interactions in a controlled environment.
- Avoid making unintended changes to your Scene while in Edit mode, as this may lead to unexpected physics simulation results.
- The tool does not automatically save your Scene. If you encounter undesired simulation results, you can revert to the previous state by reloading the Scene.
- The "Lock" feature temporarily freezes the Rigidbody properties. Performing actions unrelated to the simulation, such as removing or modifying GameObjects, while locks are active may lead to issues.
- Make sure to carefully follow the instructions and precautions provided in the tool to ensure a smooth and effective simulation experience.
