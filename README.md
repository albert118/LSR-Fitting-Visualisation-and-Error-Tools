# LSR-Fitting-Visualisation-and-Error-Tools

**A project to apply some (simple) Machine Learning to a data set**
## Overview 
<div>
  <p>The application functions in either console or windowed mode. Console mode allows desktop txt's (with console output obviously) and dataset fitting. Windowed mode utilises the <a href="https://lvcharts.net/">Live Charts library</a> to visualise the dataset.
  </p>
  <p>The dataset is built from the Guild Wars 2 (GW2) game trading system (<a href="https://api.guildwars2.com/v2">API V2</a>) to analyse the supply and demand functions of several items. In the constants class these are defined as items: <b>12429, 19697, 30699, 38131</b>.<br /> A basic URL trawler (<i>seperate program, link forthcoming</i>) gathers buy/sell price as well as quantities.
  </p> 
  <p>
    <br>The data set is analysed by a Simple Least Squares (Univariate) Linear Regression to predict the linear model of the data set of the (supply or demand) form (y = mx + b), i.e. to fit the gradient and y-intercept of the functions given the data set (<a href="https://ugess3.files.wordpress.com/2016/01/microeconomics-perloff-jeffrey.pdf"><i>See pages 14 & 19</i></a>) and are what the fitting functions fit to.
    </br> 
    <br>As the data set originally was saved to text files per item (very dirty solution) the program begins by preparing the data set from these files. Each item is created with <code>RawData</code> objects that contain the data lists; buy price, sell price, buy quantity and sell quantity. These are then trimmed to a pre-defined length via the Trimmer class and can then be weighted (Exponentially Weighted Moving Average) and/or standardised with the Model Info class. Finally, the Fitting Functions class is applied & output is generated to the desktop.
    </br>
  </p>
</div>

### Current Features:
<div>
* Model Info, Data Trimming and Fitting Functions classes.
* Text file output of the results is saved to the desktop (fitted formulas, means, modes, etc...)
* Data visualisation via the <a href="https://lvcharts.net/">Live Charts library</a>.
</div>
