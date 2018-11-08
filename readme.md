
### Table of Content
1.[Project description](html://#Project-description)

### Project description 
this project has been made in Unity to practice Neural Networks with Genetic Algortihm. 
There are 2 folders in the **Assets** folder:
 

 -**Game**: this folder is for files concerning the game itself such as bot controller script, scenes,sprites,prefabs etc
 
 **NeuralNetwork** this folder acts as a workspace for developing neural network (same files as mentioned above)

This separation was made so that other people may add their folders and fight with other neural networks in the *'Arena'*,
 in which each player places their bots with already trained neural networks (with whichever way you choose).
>more on the rules of matches here:

### The original Neural Network
As stated in the beginning the original neural network is using genetic algorithm to train the bots.
The neural network itself uses Sigmoid and SoftSign (I used 2 because they return from 0 to 1 and -1 to 1 respectively),
those functions are used to compress values to be used by the next layer.
These are the formulas:
>>>
### Sigmoid
# $` \dfrac{1}{(1+e^x)} `$

### SoftSign
# $` \dfrac{x}{(1+|x|)} `$

### The whole formula together will be as such:
# $` \dfrac{1}{(1+e^(\Sigma(w_i * x_i) + 1))} `$ 

>>>
If you want to know more about the neural networks you can read my blog post about it [here](https://steemit.com/programming/@reborninferno/day-2-or-part-2-neural-networks-and-what-you-eat-them-with)

For training i used genetic algorithm which takes the weights of the neurons and uses cross-over and mutation of random weights

###Issues
<!--stackedit_data:
eyJoaXN0b3J5IjpbMjc4NTkxMjI2XX0=
-->