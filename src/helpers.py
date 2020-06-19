# Helper functions for plotting
import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import seaborn as sns
sns.set()
pd.set_option('display.max_colwidth', None)
plt.style.use('fivethirtyeight')
plt.style.use('dark_background')
plt.style.use('seaborn-bright')

def get_reward_data(training_run, model, col1, col2, timing_data=False):
    """Gets two columns of data from imported csv files
    
    parameters:
      training_run - (list) specific training runs which correspond to folder names
      model        - (str) either Walker or Puncher
      col1         - (str) name of x column from dataframe
      col2         - (str) name of y column from dataframe
      timing_data  - (bool) will replace x data with timing data
      
    returns:
      two arrays of data
      
      """
    df = pd.read_csv(f"training_results/{training_run}/{model}.csv")
    df = df.dropna(axis=0)
    if timing_data:
        td = get_timing_data(training_run, header=None)
        for i in range(len(td)):
            td[i] = float(td[i])
        return td, df[col2]
    else:
        return df[col1], df[col2]
    
def get_timing_data(training_run, header=None):
    s = pd.read_csv(f"training_results/{training_run}/timings.csv", header=header, index_col=None)
    for i in range(s.shape[0]):
        s.iloc[i] = s.iloc[i].str.split('Time Elapsed: ')[0][1].split(' s')[0]
    return s[0]

def single_plot(x, y, xlabel, ylabel, color, model):
    """Used to plot a singe x-y pair
    
    parameters:
      x      - (list or array) x axis data
      y      - (list or array) y axis data
      xlabel - (str)  label used for x axis
      ylabel - (str)  label used for y axis
      color  - (str) plot color
      model  - (str) either Walker or Puncher
      
    returns:
      none
      
      """
    fig, axs = plt.subplots(figsize=(12,10))
    axs.plot(df1_x, df1_y, color=color)
    axs.grid(False)
    #axs.legend(fontsize=20)
    axs.set_xlabel(xlabel)
    axs.set_ylabel(ylabel)
    axs.set_title(f"{ylabel} vs {xlabel} for {model}")
    plt.show()
    
def plot_multiple(training_runs, labels, model, col1, col2, figsize=(12,10), savefig=False, save_name='images/Testplot'):
    """Plots multiple plots in one figure. Has the option to save the figure if savefig is True
    
       parameters:
         training_runs - (list) training run names (strings) which are the names of the 
           folders in the training_results folder
         labels        - (list) label names for plots
         model         - (str) either Walker or Puncher
         col1          - (str) the name of the column for the x axis data
         col2          - (str) the name of the column for the y axis data
         figsize       - (tuple) the size of the plot
         savefig       - (bool) the figure will be saved if savefig is set to True
         save_name     - (str) the path and name of the figure to be saved
         
       returns:
         none, will plot the figure and save it if requested.
         
         """
    fig, axs = plt.subplots(figsize=figsize)
    for i in range(len(training_runs)):
        df_x, df_y = get_reward_data(training_runs[i], model, col1, col2)
        axs.plot(df_x, df_y, label=labels[i])
    axs.grid(False)
    axs.legend(fontsize=20)
    axs.set_xlabel(col1)
    # clean up the names a little
    if col2 == 'Policy/Extrinsic Reward':
        col2 = 'Policy Reward'
    elif col2 == 'Policy/Entropy':
        col2 = 'Policy Entropy'
    axs.set_ylabel(col2)
    axs.set_title(f"{col2} vs {col1} for {model}")
    if savefig:
        plt.savefig(f'{save_name}.png')
    plt.show()
    
def plot_multiple_timing(training_runs, labels, model, col1, col2, figsize=(12,10), savefig=False, save_name='images/Testplot', timing_data=True):
    """Plots multiple plots in one figure where training time is the x axis. 
       Has the option to save the figure if savefig is True
    
       parameters:
         training_runs - (list) training run names (strings) which are the names of the 
           folders in the training_results folder
         labels        - (list) label names for plots
         model         - (str) either Walker or Puncher
         col1          - (str) the name of the column for the x axis data
         col2          - (str) the name of the column for the y axis data
         figsize       - (tuple) the size of the plot
         savefig       - (bool) the figure will be saved if savefig is set to True
         save_name     - (str) the path and name of the figure to be saved
         timing_data   - (bool) passed into the `get_reward_data()` function
         
       returns:
         none, will plot the figure and save it if requested."""
    fig, axs = plt.subplots(figsize=figsize)
    for i in range(len(training_runs)):
        df_x, df_y = get_reward_data(training_runs[i], model, col1, col2, timing_data=True)
        axs.plot(df_x, df_y, label=labels[i])
    axs.grid(False)
    axs.legend(fontsize=20)
    axs.set_xlabel('Total Training Time (s)')
    # clean up the names a little
    if col2 == 'Policy/Extrinsic Reward':
        col2 = 'Policy Reward'
    elif col2 == 'Policy/Entropy':
        col2 = 'Policy Entropy'
    axs.set_ylabel(col2)
    axs.set_title(f"{col2} vs Total Training Time for {model}")
    if savefig:
        plt.savefig(f'{save_name}.png')
    plt.show()