
## Table of Content
1. [Project description](#project-description)
2. [Game mechanics](#game-mechanics)
3. [The original Neural Network](#the-original-neural-network)
   * [Sigmoid](#sigmoid)
   * [SoftSign](#softsign)
   * [The Whole Formula](#the-whole-formula-together-will-be-as-such)
   * [Interaction-of-NN-with-the-bot](#interaction-of-nn-with-the-bot)
      * [Inputs]()
      * [Outputs]() 
    * [Issues](#issues)
4. [Creation Guidelines](#creation-guidelines)
5. [Match Guidelines](#rules-for-the-matches)
### Project description 
this project has been made in Unity to practice Neural Networks with Genetic Algortihm. 
There are 2 folders in the **Assets** folder:
 

 - [**Game**](#game-mechanics): this folder is for files concerning the game itself such as bot controller script, scenes,sprites,prefabs etc   
 - [**NeuralNetwork**](#the-original-neural-network) this folder acts as a workspace for developing neural network (same files as mentioned above)

This separation was made so that other people may add their folders and fight with other neural networks in the *'Arena'*,   
in which each player places their bots with already trained neural networks (with whichever way you choose).  
I also hope that by seeing this project and using it as an example, people would make their own networks to fight in this arena ;).
>more on the rules for matches [here:](#rules-for-the-matches)

### Game Mechanics
In this section there will be all needed information about the bot and it's possible actions,
more information about how the network interacts with the bot  is [here](#interaction-of-NN-with-the-bot)
the bot variables are:
>>>
* `int health`: This variable will start at 3 and when it reaches 0 the bot dies.  
* `float viewAngle`: This variable determines the view angle of the bot, Within this angle the bot can see the enemy and `IsEnemyInView ` will be true.        
* `bool isEnemyDashing`: This is to show if enemy is currently dashing or not (arbitrary from the view Angle).
>>>

There are a number of inputs(actions) that the bot can do: these are as follows:

1. Dash Forwards, This is the main for of attack. If you touch your opponent during the dash it will get damaged.
2. Dash Backwards, Same as above but opposite direction.
3. Rotate, this is done using one action depending if the value is negative or positve (rotating left or right respectively).
4. Change view angle size, same as aboe but instead of rotation it's changing the size of the viewing angle (max is 360 of course)

### The original Neural Network
As stated in the beginning the original neural network is using genetic algorithm to train the bots.
The neural network itself uses Sigmoid and SoftSign (I used 2 because they return from 0 to 1 and -1 to 1 respectively),
those functions are used to compress values to be used by the next layer.
These are the formulas:
>>>
#### Sigmoid
# $` \dfrac{1}{(1+e^x)} `$

#### SoftSign
# $` \dfrac{x}{(1+|x|)} `$

#### The whole formula together will be as such:
# ` ![\Large x=\frac{1}{(1+e^(\Sigma(w_i * x_i) + 1))}](https://latex.codecogs.com/svg.latex?x%3D%5Cfrac%7B-b%5Cpm%5Csqrt%7Bb%5E2-4ac%7D%7D%7B2a%7D) `

![\Large x=\frac{-b\pm\sqrt{b^2-4ac}}{2a}](https://latex.codecogs.com/svg.latex?x%3D%5Cfrac%7B-b%5Cpm%5Csqrt%7Bb%5E2-4ac%7D%7D%7B2a%7D)
>>>
If you want to know more about the neural networks you can read my blog post about it [here](https://steemit.com/programming/@reborninferno/day-2-or-part-2-neural-networks-and-what-you-eat-them-with)

For training i used genetic algorithm which takes the weights of the neurons and uses cross-over and mutation of random weights

### Interaction of NN with the bot
Here will be shown the output input values that network to do calculations

##### Inputs of the network
* `isEmenyInView`: is the enemy in view
* `isEnemyDashing`: is the enemy dashing
* `viewAngle`: size of the view Angle
* `enemyDistance`: how far away is the enemy

##### outputs of the network
> These are propeties for the buttons inputs of the bot (read only)
* `RotationButton `: responsible for rotation of the bot
* `ViewAngleChangeButton `: responsible for change of the angle size
* `DashButton`: responsible for dashing action
* `DashBackButton `: responsible for dashing backwards

### Issues
The network itself if finished, and the algorithm works, how ever there are some bugs with the algorithm thus preventing it from futher learning. And the network itself is not flexible (As in able to be made into any size).
These issues are easy to solve however i had limited time to finish this and i will add and fix what is needed when i will have time.

### Creation guidelines
If you want to create your own network then you would need to follow these steps:

1. Create a gitlab account if you don't have one.
2. Create a new branch from the master.
3. Create a separate folder with your nickname in the beginning to identify it as yours
   > Make sure that the name doesn't conflict with anyone elses.
4. Start by connecting your network with the bot Using the properties that are included inside.
5. You can do whatever you want in your folder but do not edit anything outside of it.

There are also some Rules you need to follow otherwise your merge request will be declined,
* You must never edit `Game` folder, if you have anything to change and you think it is necessary,   
  create an issue and i will evaluate on if it is needed or not.
* If your version is too old compared to the master branch you will need to update your Unity before requesting to merge.
* Your saved weights must be saved as a txt file (or json if you prefer),  
 you're required to do that if your network is working and doesn't have any bugs in it

### Rules for the matches
When a match is organized (by creating an issue), each opponent needs to pepare their network by doing these steps:
* Separate your network and training
* Save your weights to a file (txt/json)
* save these files into a separate folder and place it into your workspace folder


<!--stackedit_data:
eyJoaXN0b3J5IjpbMjc4NTkxMjI2XX0=
-->
