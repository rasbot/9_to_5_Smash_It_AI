<!-- PROJECT SHIELDS -->
<!-- [![LinkedIn][linkedin-shield]][linkedin-url] -->

<p align="center">
  <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/punchy2.png" width="1600" height="auto"/>
</p>

<!-- PROJECT LOGO -->
<br />
<!-- <p align="center">
  <a href="https://github.com/rasbot/Reinforcement_Learning_in_Unity">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/robot_fail.gif" alt="Logo" width="600" height="auto">
  </a>

  <h3 align="center">Reinforcement Learning in 3D Simulated Environments</h3> -->

<!--   <p align="center">
    Using Unity to simulate environments to train agents using reinforcement learning.
    <br />
    <a href="https://github.com/rasbot/Reinforcement_Learning_in_Unity"><strong>Explore the docs »</strong></a>
    <br />
    <br />
    <a href="https://github.com/rasbot/Reinforcement_Learning_in_Unity">View Demo</a>
    ·
    <a href="https://github.com/rasbot/Reinforcement_Learning_in_Unity/issues">Reinforcement_Learning_in_Unityrt Bug</a>
    ·
    <a href="https://github.com/rasbot/Reinforcement_Learning_in_Unity/issues">Request Feature</a>
  </p>
</p> -->


## Table of Contents

* [Introduction](#introduction)
* [Project Scope](#project-scope)
* [Reinforcement Learning Algorithms](#reinforcement-learning-algorithms)
  * [Proximal Policy Optimization](#proximal-policy-optimization)
    * [Hidden Layers](#hidden-layers)
    * [Learning Rate](#learning-rate)
    * [Summary Frequency](#summary-frequency)  
    * [Number of Parallel Agents Training](#number-of-parallel-agents-training)
    * [Comparing Tuned Model to the Default](#comparing-tuned-model-to-the-default)
  * [Soft Actor Critic](#soft-actor-critic)
* [Walking Trainer](#walking-trainer)
  * [Untrained Walker](#untrained-walker)
  * [PPO Training](#ppo-training)
  * [SAC Training](#sac-training)
  * [Extensions](#extensions)
* [Puncher Trainer](#puncher-trainer)
  * [Rewards](#rewards)
  * [Scaled Down Version](#scaled-down-version)
  * [Comparing Two Models with Different Rewards](#comparing-two-models-with-different-rewards)
  * [Rebuilding Game Environment](#rebuilding-game-environment)
* [Next Steps](#next-steps)
* [Contact](#contact)
* [Acknowledgements](#acknowledgements)


## Introduction
---
Reinforcement learning has many useful applications such as robotics and self-driving cars. Researchers in artificial intelligence (AI) are constantly improving learning algorithms, and benefit from the ability to train and test their models in different environments. By enabling testing and training within a simulated environment, algorithms can be improved more rapidly. These ideal environments have a physics engine as well as graphics rendering. This is a benefit as well for companies that need to test and deploy trained models in areas like robotics. Having a fully simulated robot within a simulated environment allows for rapid and cost-effective testing before testing in a real environment with a real robot.

Machine learning (ML) models depend on what type of learning or predictions are involved.

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/ML_map.PNG" width="400" height="auto"/>
</p>

If the model will make a prediction of a specific target or label from a set of features, supervised learning can be used. If the data is unstructured or unlabeled, and the target feature is not known, unsupervised learning can be used to find relations between features. If the goal is to have a model learn from the environment through interaction, reinforcement learning is used.

Reinforcement learning involves an __agent__, which could be a robot, a self-driving car, or a video game character, that interacts and learns from the __environment__. The agent observes the environment, takes __actions__, and receives __rewards__ based on those actions. The reward could be positive or negative, such as if a robot moves closer to its target it would get a positive reward, and if it moves away from its target it would get a negative reward. The goal of the model is to maximize the reward to perform a specific task. An agent might not ever receive a positive reward but get penalized such as an agent navigating a maze. The penalty might be a negative reward for each step and the model trains the agent to minimize the penalty. The algorithm used to train the model is called the __policy__. This will be discussed more later.

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/agent_env.PNG" width="500" height="auto"/>
</p>

The Unity game engine provides an editor which allows for ML algorithms to interact and learn from a variety of simulated environments. Recently the Unity team released version 1.0 of their [ML Agents toolkit](https://github.com/Unity-Technologies/ml-agents). This toolkit, along with the Unity editor, provides a user with the ability to create 3D simulated environments, a Python API to control and integrate ML algorithms into the environments, and a learning pipeline using C# to collect data from the environment, implement agent actions, and collect rewards. Agents can be trained and tested in the Unity engine using the ML agents toolkit.

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/Unity_pipeline.PNG" width="600" height="auto"/>
</p>

<p style="text-align: center;">
The Unity Learning Environment
</p>

The Python API interfaces with the Unity game engine to create and control ML algorithms in the simulated environment. Python code initializes the environment variables, and feeds agent interactions to the model during training. The trained model is then fed back into the agent during testing.

Environments and game objects are created in Unity giving full control over the physics of the environment.

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/unity.PNG" width="800" height="auto"/>
</p>

## Project Scope
---
In 2018 I published a mobile game to the Google Play store called `9 to 5 Smash It`. 

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/9to5.gif" width="700" height="auto"/>
</p>

The game was built in the Unity game engine and coded in C#. The game mechanics use a `magic window` which uses a modified GoogleVR APK to track the phones rotation. This gives 3 degrees of freedom which the player can use to move their phone around to see the game world in 360 degrees. The player can do a punch attack by pushing their phone forward, using the phones accelerometer to scale the attack. The harder the player punches the phone forward, the larger the magnitude of the attack in the game. 9 to 5 Smash It is set in an office environment, where the player needs to smash their way through the competition to climb the corporate ladder. Enemies are office-bots which the player needs to punch until they are destroyed. 

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/smash_it.gif" width="700" height="auto"/>
</p>

The end goal of this project is to train the office-bots to walk and seek out the player, and to train the player to seek out bots and destroy them using reinforcement learning. Using Unity will allow the ability to feed in the specific inputs to the neural net used in deep reinforcement learning. The game objects will be trained within the engine, and the trained models will be deployed in the engine to control the characters. Two algorithms were used to train the walkers, which will be discussed below. 

## Reinforcement Learning Algorithms
---
As mentioned above, the policy is the algorithm used in reinforcement learning (RL). The simplest example, which doesn't involve any learning, is a stochastic policy where an agent randomly moves around an environment. If the goal is to do something like collect all the coins in an environment, a random walk will eventually get the job done. If the policy has feedback from the environment, a more intelligent policy can be adopted.

Reinforcement learning has been used to play simple video games, and improvements to RL algorithms has given researchers and developers a richer ability to train agents to navigate game environments. 

### Proximal Policy Optimization
Proximal policy optimization (PPO) was released in July 2017 by OpenAI. PPO is a deep learning algorithm that balances easy implementation and tuning while minimizing the cost function and evolving the policy in a "safe" manor, by only making small changes to the previous policy. The agent takes actions within the environment and collects rewards associated with those actions. This is done in batches, and after each batch, the gradient of the reward is calculated, and the policy is updated. The batch is thrown out since the policy is updated, and the process is repeated. By not having to store all the batch training information, PPO is more efficient than previous deep reinforcement learning algorithms. 

### Soft Actor Critic
Soft Actor Critic (SAC) seeks to maximize the entropy of the policy. Instead of just playing it safe like PPO, SAC takes greater variation on actions to attempt to maximize the reward. By being more erratic, newer ways to maximize the reward can be found by taking a larger risk. SAC takes longer to train compared to PPO for the same number of epochs, and both policies will be compared.

## Walking Trainer
---
Currently in 9 to 5 Smash It, the bots were 3D modeled to not have legs and instead have wheels or are able to hover. When developing the game, the navigation script to control the bots was all hard-coded behavior. The script allowed the bots to turn toward the player and navigate to them. Obstacle detection was also implemented so the bots could move around things in a believable way. The movements of the bots were animated in the game editor and the script controlled the animations. 

In order to have the robots walk, previous 3D models of the bots would have to be used and they would have to be rigged and animated to walk. For this part of the project, reinforcement learning will be used to train an agent to walk. The Unity ML Toolkit has a walking simulator which was used as a base to train the behavior of walking. The walkers were given rewards for moving forward, and penalties for falling over. After training the walkers, they should be able to walk forward without falling over. After training the models would be integrated into the bot models with legs.

<!-- UNTRAINED WALKER -->
### Untrained Walker
When a walker is untrained, it will just fall over. It has no ability to control the limbs of the walker body. It is not even being told to try to move around since it is not being trained yet.

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/UNTRAINED.gif" width="600" height="auto"/>
</p>

Let's look at how the training process goes for both PPO and SAC.

<!-- PPO WALKER -->
### PPO Training
Training the walkers was done with several agents being trained in parallel. During training, the agents are trying to see how to move in order to maximize their reward for each training batch. For these plots, the `Step Number` is the number of training steps the policy has gone through, and the `Policy Reward` is the total reward for the model as it trains.

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/training.gif" width="600" height="auto"/>
</p>

In order to attempt to increase the performance of the model, some of the hyperparameters were tuned. The optimal value of each is related to the value which produces the largest policy reward over the training time.

#### Hidden Layers
When looking at the hidden layers within the neural net, equal training batches with different numbers of hidden layers were compared.

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/reward_layers.png" width="700" height="auto"/>
</p>

For these runs, training times were around 1 hour. It appears that using one hidden layer is optimal for this amount of training since the policy reward was higher for the entire training session.

Another important metric to look at is the policy entropy. The policy's goal is to increase the reward over the training time, but it is also trying to decrease entropy. Lower entropy means that the policy is more sure about which actions to take to increase the reward.

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/entropy_layers.png" width="700" height="auto"/>
</p>

Using this metric, it appears that 2 hidden layers does a better job of decreasing entropy. Both reward and entropy are important to consider when deciding on an optimal value for a particular hyperparameter.

#### Learning Rate
The learning rate of the model was also examined. 

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/reward_lr.png" width="700" height="auto"/>
</p>

If the learning rate is too small, the policy will be updated very slowly. If the learning rate is too high, the model will be overfit and will have trouble converging on the maximal policy reward.

This is evident in the higher values of the learning rate. Both the `0.03` and `0.008` trainings show erratic policy rewards as the policy is updated. A learning rate of `0.003` seemed to be optimal here.

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/entropy_lr.png" width="700" height="auto"/>
</p>

As expected, the higher learning rates that overfit the training data actually increase in entropy during training. This makes sense because they are updating the policy too quickly and instead of converging on higher policy rewards, they cause wild fluctuations as the policy is trying erratic actions instead of honing in on the actions that will increase the reward. This plot reinforces the optimal learning rate of `0.003` since it not only increases the policy reward the most, it decreases entropy the most as well.


#### Summary Frequency
The summary frequency was compared for different training runs as well. This is essentially how often the policy is updated. If the summary frequency is low, the policy will update more often, and the policy reward value will fluctuate compared to a lower summary frequency where the update occurs with larger batches and is generally smoother.

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/reward_freq.png" width="700" height="auto"/>
</p>

It appears that a summary frequency of `10000` looks optimal since it has a high policy reward and is less erratic than `3000`. How does the entropy plot look?

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/entropy_freq.png" width="700" height="auto"/>
</p>

It looks like `20000` is most likely the optimal value for summary frequency since it is smooth (less erratic), maximizes the policy reward fairly well, and has the lowest entropy compared to other values.

#### Number of Parallel Agents Training
This one is not exactly a hyperparameter, but since the agents can be trained in parallel, the number of agents training was compared. The intuition here is that the more agents that are trained in parallel, the better trained the model will be after some amount of training time. If too many agents are training it could bottleneck training due to the CPU/GPU not being able to handle the number of agents.

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/reward_time_agents.png" width="700" height="auto"/>
</p>

Looking at the entropy,

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/entropy_time_agents.png" width="700" height="auto"/>
</p>



These training runs were all of equal step number. This shows that by training more agents, the training time is reduced, and it appears that for 3, 6, and 9 agents that the reward is proportional to the number of agents. However, when training 30 agents, there is no increase in performance. This could be due to the bottlenecking issue mentioned above.

#### Comparing Tuned Model to the Default
Comparing training between the tuned model and the default gave what appeared to be slightly better results for the tuned model.

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/reward_2runs_trunc.png" width="700" height="auto"/>
</p>

The tuned model appears to be performing better than the default for most of the trainings. However, they seem to converge around the same reward value near the end. Increasing training time for both models gave more telling results.

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/reward_2runs.png" width="700" height="auto"/>
</p>

Clearly this shows that the default model did much better than the tuned model. One reason is most likely due to the default model having a learning rate that was an order of magnitude larger than the tuned model. In shorter training times it appeared that the selected learning rate was ideal. With longer training, this shows that there are non-linear effects that are not accounted for with shorter training times. It could also be that the hyperparameters are coupled and that combinations of hyperparameter values need to be explored.

The take-home message is that much longer training times need to be explored while tuning the model.

<!-- SAC WALKER -->
### SAC Training
SAC training was much longer than PPO. Training time is a cost to companies, so considering how these two policies compare is a balance between performance and training time. After training an agent with SAC for around 8 hours, the results were not impressive. 

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/3models.gif" width="600" height="auto"/>
</p>

PPO (long) and SAC were trained for the same number of steps. PPO (short) was trained for significantly less steps and does about as good as SAC. Directly comparing the performance of the PPO (long) and SAC training shows which model tended to do a better job of training the walkers.

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/reward_PPO_SAC.png" width="600" height="auto"/>
</p>

You can see how SAC tends to be more erratic as it increases entropy and is more risky in its policy changes than PPO. PPO is playing it safer, and the policy reward updates tend to be smoother and slower to change. Clearly PPO did a better job for the same number of steps...but what about training time?

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/time_PPO_SAC.png" width="600" height="auto"/>
</p>

When looking at training time vs step number, SAC took much longer to complete the training. 

Looking at the policy reward over time shows that PPO did a much better job finding actions that increase the reward compared to SAC. If training times are not important, one could argue that SAC ended up doing better at maximizing the policy reward, but if the step number for PPO was increased so it trained for longer times, it appears that PPO might still perform better overall.

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/reward_time.png" width="600" height="auto"/>
</p>

The policy reward increased smoothly and quickly for PPO
For the walkers and punchers, PPO seems to be the optimal policy.

### Extensions
Since the agents are trained in the environment and will learn to take actions that increase the reward, changing the features of the agent can be done before training. In this example, limb length was increased to create a lanky agent. Using PPO, the agent was trained to walk for the same training time that previous models were trained on. 

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/LANKY.gif" width="600" height="auto"/>
</p>

After training, the lanky model had more trouble walking due to the center of gravity being higher so the body experienced more torque and was less stable than the standard walker. 

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/lanky_boi.gif" width = "700" height="auto"/>
</p>

Looking back at the PPO trained standard walker, I wanted to look at how the model adapted to a change in the mass distribution of the body after it was trained. 3 trained walkers were placed next to each other. The left model has no changes to the body. The mass of the left hand was increased in the Unity engine for the middle and right models by 20x and 100x, respectively. The hand sizes were increased for visual clarity, even though the mass of the body part is not coupled with the size of the part.

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/fathands.gif" width = "700" height="auto"/>
</p>

The model seems to try to compensate for the uneven mass distribution by pulling the hand closer to the center of mass. The model would perform much better if it was trained with the different agents, but it is interesting to see how the trained model adapts to this change.

## Puncher Trainer
---
The player character is more challenging to train. The player needs to look around the game environment and identify enemy bots. Once identified, the player has to punch the bot until it is destroyed, while trying to prevent any bots from reaching the player, as that lowers the players health bar. The reward system needs to be outlined before the puncher training can begin. 

Writing the scripts in C# to read in the right variables to feed into the neural net was tricky. During training, the time scale of the environment is increased to about 10x. This can cause issues with the physics of the game. The physics will update every frame, and if the animation for the puncher is sped up and moves too fast, the collision detection will not register a hit. Training game objects to interact with other game objects via collision detection will suffer from this problem. One way around this for testing would be to register a hit if the agent was looking directly at a target while the animation to punch was triggered. We could then safely assume a hit should be registered.

### Rewards
Setting up the right rewards is tricky. It takes several iterations to understand how to implement proper rewards and penalties to get the agent to do what is expected. The areas where rewards were initially given were:

* Smallish reward for punching an enemy.

* Largish reward for destroying an enemy.

Seems simple enough, but the agent also needs to be encouraged to find enemies quickly. This means that time-based rewards are given for looking for the enemy.

* Small time-based penalty for not seeing an enemy.

* Slightly larger time-based reward (with respect to the magnitude of the time-based penalty) for seeing an enemy.

The process used to detect an enemy will be discussed below. To integrate the machine learning training into the game environment, I had to start smaller and create a minimal agent and environment.

### Scaled Down Version
The basic game mechanics are coded into this simplified environment. The puncher agent has a cube object that is animated in a similar way to act as the punching glove (referred to as the puncher cube). The punch animation is triggered, and the cube will move outward and back. The punch cube has a box collider attached to it, and uses logic that will check if a collision was made with the enemy cube (by checking the tag on the game object to see if it is equal to "Target"). If it hits the cube, it will increment a counter and once that counter reaches 5 hits, the target cube will be destroyed.

Very early tests showed some issues that needed to be resolved, like clamping the puncher agents rotations in unintended directions.

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/early_training.gif" width = "700" height="auto"/>
</p>

Each training session starts with the target cube being randomly placed in the game environment, and the puncher then tries to locate and punch it. The actions available to the puncher agent will be described below.

The inputs to the neural net are:

* The position of the target cube.

* The hit count of the puncher cube.

* The look count of the puncher agent (this is a counter that increases with time as the puncher agent "sees" the target).

* A bool which is set to `True` if the puncher cube is colliding with the target, and `False` when it is not.

* The rotational position of the puncher agent about the vertical axis.

* For later (not the simplified version), the velocity of the targets in both `x` and `z` where `y` is the vertical direction.

The actions that the agent can take during training, which it will take to try to get rewards, are:

* Trigger the punch attack.

* Rotate the agent about the `y` direction in a + or - direction.

No actual values are given to the agent, it will try different values for each action and figure out which values will increase the policy reward.

Training 9 agents in parallel was done for about 6 hours:

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/training_9.gif" width = "700" height="auto"/>
</p>

The results of the initial training were promising:

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/early_test_better.gif" width = "700" height="auto"/>
</p>

The agent appears to try to seek out the target cube and does try to punch it. Not too bad! At this point I wanted to encourage the agent to seek the target cube faster. I increased the time-based reward for being able to see an enemy, and after letting it train all night to my dismay, the agent did not perform better. In fact, since the agent gets a time-based penalty for not seeing the target, a time-based reward for seeing it, a smallish reward for punching it, and a larger reward for destroying it...the agent was able to maximize the policy reward by simply looking at the static target. By continuing to look at it, it got rewarded whereas if it destroyed it, it would have to start looking around for the next target and be penalized. The agent learned to maximize the reward, but not in the way I had intended it to do so. 

### Comparing Two Models with Different Rewards
The values associated with the rewards have been vauge so far in that I have referred to them in a relative magnitude. There is an arbitrary aspect to the actual values because I could assign a reward of 1 for an action, or 1000. The relative magnitudes of the rewards, as they relate to each other is what is important. I trained two models and looked at how the training compared. The model rewards are:

Model_1 :

* Hitting a target : 0.2

* Destroying a target : 1.0

* Seeing a target : 1/60 per frame

* Not seeing a target : -1/60 per frame

Model_2:

* Hitting a target : 0.2

* Destroying a target : 1.0

* Seeing a target : __2/60__ per frame

* Not seeing a target : -1/60 per frame

Note that if the game runs at 60 frames per second, these time-based rewards can be understood as a time frequency instead of a frame frequency.

Looking at how the models compared is tricky because if the reward system is altered, direct comparisons to policy reward will not make sense. For instance,

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/reward_2runs_puncher.png" width="700" height="auto"/>
</p>

A larger net reward can just be a factor of the increase in reward magnitude for performing specific actions. A more telling metric would be to compare the policy entropy between the models.

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/entropy_vs_steps_23.png" width="700" height="auto"/>
</p>

Looking at this plot, the second model appears to have lower policy entropy for the same number of steps. The entropy will go down as the model becomes more sure of which actions will lead to a maximal policy reward. It appears that the increase in the positive time-based reward for looking at a target has increased the model's performance slightly.

### Rebuilding Game Environment

After training a model in a simplified environment, adding elements of 9 to 5 Smash It was done to start building up to the game. The player in the game plays in first person mode so there is no actual game object for the player character. The player has a glove on a spring that is used to punch and for sake of being able to see the player character during training and testing, a bot from the game was used for the player character. The bot has the glove attached so it can punch.

The enemies in the game move toward the player and do damage when they make contact with them. As a first step the target cube was replaced with a game character as well, and the environment skybox from the game was added as well.

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/coffee_bot_test.gif" width="800" height="auto"/>
</p>

When watching the trained model, a few things stand out.

1. The agent tends to spam the attack button. This is actually a decent strategy in a video game if there is no penalty for attacking too much. In the real world we need to optimize our energy output and attacking when not hitting the target is not efficient. In this case there is no penalty to keep attacking and much like myself when I play some games, mashing the attack button does get the job done.

2. The agent seems to constantly rotate which is not a terrible strategy as well. A real player can see more than what is directly in front of then and peripheral vision can help us know which way to turn. We also have other sensory inputs like hearing that helps us to quickly turn in the direction of an event. Fine tuning the inputs into the neural net and incentivizing quicker movements would help this process.

3. The agent sometimes turns past the target, and even after hitting it sometimes. This could be helped with changing the rewards given after a successful punch.

The idea here is to add multiple targets to the environment. The targets will not be static, and they will use the walker models for locomotion. One of the 9 to 5 Smash It levels is used for the next steps here, and the walker agents are being used as targets.

<p align="center">
    <img src="https://raw.githubusercontent.com/rasbot/Reinforcement_Learning_in_Unity/master/images/FullTest.gif" width="800" height="auto"/>
</p>

This is a little wonky, but it is going in the right direction. This test was done with different walker agents including the lanky one. The trained walkers do not walk very well in this environment despite the fact that they are just trying to move toward the player. The player is set as their navigation target, but they perform a lot less effectively in the office environment than they do in an open plane. Maybe the models prefer to work remotely instead of coming into the office!

## Next Steps
---
Next steps for this project will include recursively training the enemy bot walkers and the player puncher to see if they can learn from each other.

For the walkers:

* Add legs and rig the models so they can use the walker models.

* Additional aspects can be hard coded into the enemy bots such as the ability to turn and look at the player, as well as simple navigation to move in the direction of the player. 

* Train the walkers in the 9 to 5 Smash It environment. Obstacle avoidance should be part of the training.

For the punchers:

* Fix the issue with collision detection. This can be done by changing how hits are detected.

* Fine tune the reward system, which will take many training sessions to understand each change.

* After updating the reward system, allow for detection of multiple enemy bots. Retrain with moving bots.

* Tune model hyperparameters which will take several training sessions to understand each change.

## Conclusion
---
Unity is a great engine to create simulated environments as well as simulated agents to train machine learning models on. The Unity team's ML Toolkit is a great way to create a pipeline from the Unity engine, through C#, into their Python API to train and deploy models. This can be extremely useful for robotics, self-driving cars, and video games. Working with Unity and the ML Toolkit was impressive, and I look forward to what the Unity team has in the future for machine learning in simulated environments!

## Contact
---
Nathan Rasmussen - nathan.f.rasmussen@gmail.com

Linkedin: [https://www.linkedin.com/in/nathanfrasmussen/](https://www.linkedin.com/in/nathanfrasmussen/)

Unity Connect: [https://connect.unity.com/u/nathan-rasmussen](https://connect.unity.com/u/nathan-rasmussen)

Project Link: [https://github.com/rasbot/Reinforcement_Learning_in_Unity](https://github.com/rasbot/Reinforcement_Learning_in_Unity)



<!-- ACKNOWLEDGEMENTS -->
## Acknowledgements
---
* [Galvanize Data Science](https://www.galvanize.com/data-science-bootcamp) 
* [Unity ML Toolkit](https://github.com/Unity-Technologies/ml-agents)
* [Martian Games](www.martiangames.com)





<!-- MARKDOWN LINKS & IMAGES -->
<!-- https://www.markdownguide.org/basic-syntax/#reference-style-links -->

<!-- [linkedin-shield]: https://img.shields.io/badge/-LinkedIn-black.svg?style=flat-square&logo=linkedin&colorB=555
[linkedin-url]: https://linkedin.com/in/nathanfrasmussen -->