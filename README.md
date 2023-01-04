# DCSFramework

This is an OOP unity game framework.

The framework enforces a separation of data and logic. For each system, we need to define the data and authority(readonly/readwrite). Then during runtime, this framework will send the corresponding data to system based on the declaration. So we can protect our data during runtime and allow us to run the gameplay in multithread.

This framework also separates visual and logic. Visual is like model proxy, animation proxy, etc. And all logic can only use visual by visual command and visual proxy id.

### Modules
- GalaxyEntry: game framework entry
- GCore\Base: basic game framework module
- CommandManager: dispatch commands based on entity unique id
- GalaxySystemManager: all system instances will save here
- EntityManager: manage all the entity life cycle. and the entity will send its data to the system they want to use
- EventManager: manage and send all the event
- HolderManager: manage all the entity data. the entity can use its unique id to get the data holder from this manager
- HolderProxy: the proxy for each system/command that contains the data types and permission they need. This can optimize performance in the runtime
- MoldMain: call the mold generator to generate code based on GalaxyAttribute
- ProcedureManager: manage the entire game flow
- VisualProxyManager
    1. Visual manager: manage all the visual entity lifecycle
    2. Visual proxy: work between the gameplay side and the visual entity. We can use the visual proxy to do some gameplay operations even though we haven't finished loading the resource. All gameplay call will be saved to a queue, once the resource is loaded, we will handle them in order. It will reduce complexity and help us to handle special cases.
    3. Visual cmd/query: the way to communicate with the visual layer. Every time we call visual cmd, it will be added to a queue on the visual proxy. Then they will be handled once the visual entity is ready.
    4. Example: we want to create a shooter enemy. First we need to load the enemy model, then we need to attach the gun to the enemy's right hand. In this case, we can just create the enemy model and gun model. Then call the Attach function to move the gun model to the enemy's right hand. And call visual commands to set those positions, rotations, etc. We can do all the above operations in one shot. Then in the runtime, it will work like below:
        1. After loading the enemy model, the system will handle the flush queue to set position, rotation, etc.
        2. The system will try to find out all the attached commands related to this model. In this case, it will find the gun's attached command.
        3. Check whether the gun model is ready or not. If not, then wait for it to be ready.
        4. After loading the gun model, the system will perform similar steps like the above 1-3, until all attached commands have been finished.
