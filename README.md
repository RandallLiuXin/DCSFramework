# DCSFramework

This is an OOP framework. The framework enforces a separation of data and logic. And different system need to define the data they need and define the authority(readonly/readwrite) they need. Then when we run the logic, this framework will send the corresponding data to logic based on the declared type. So we can protect our data during the update and give our chance to run the gameplay with multithread

And we also separate visuals and logic. Visual include model proxy, and animation proxy, etc. And all logic can only use visual by visual command and visual proxy id.

### Modules
- GalaxyEntry: game framework entry
- GCore\Base: basic game framework module
- CommandManager: dispatch commands based on entity uid
- GalaxySystemManager: all logic object will save here
- EntityManager: manage all the entity life cycle. then entity will send its data to real logic
- EventManager: manage and send all the event
- HolderManager: manage all the entity data. entity will use its uid to get the holder from this manager
- HolderProxy: the proxy for each system which contains the data types and permission they need. This can optimize performance in the runtime
- MoldMain: it will call the mold generator to generate code based on GalaxyAttribute
- ProcedureManager: manage the entire game flow
- VisualProxyManager
    1. Visual manager. we use this to manage all the visual entity lifecycle
    2. Visual proxy. it works between the gameplay side and the visual entity. for example, if you create a new enemy, we need to create an enemy model and animator, they all need some time to load, so we always need to add lots of async operations into the callback. but it is tricy and we need to add some logic to handle special case. this is why we need the visual proxy. you can use the visual proxy to do your gameplay logic even though we haven't finished loaded. and all your gameplay call will be added to a queue and handle after the res loaded.
    3. Visual cmd/query is the way you can communicate with the visual layer. every time you call visual cmd, it will be added to a queue on the visual proxy. then they will be handled when the visual entity is ready.
    4. For example, we want to create a shooter enemy. So we need to load the enemy model, then we need to attach the gun to the enemy's right hand. In this case, you can just create the enemy model and gun model. Then attach the gun model to the enemy's right hand. And call visual commands to set those positions, rotations, etc. The flow should be like this:
        1. After finishing loading the enemy model, we will handle the flush queue to apply command.
        2. Then try to find out all the attached commands related to this model. in this case, will find the gun's attached command
        3. After finishing loading the gun model, we will handle the gun's flush queue
