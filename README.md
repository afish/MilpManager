# MilpManager
Library for Mixed Integer Linear Programming. It contains abstraction for pluggable solvers, and implementation of common mathematical functions.

# What is MilpManager
This library simplifies the process of creating ILP programs. It gives a fluent interface to define variables, constraints, and cost function.

# How to use MilpManager
This package contains only implementation of common mathematical functions (e.g., max, min, multiplication, sorting etc.). In order to create an actual problem and solve it, you need to use concrete implementation (like CplexMilpSolver https://github.com/afish/CplexMilpSolver).

## Examples
First you need to create solver. Let's use CPLEX implementation:
```
var solver = new CplexMilpSolver(10);
```
Now we are able to define variables:
```
IVariable x = solver.Create("x", Domain.BinaryInteger);
IVariable y = solver.Create("y", Domain.PositiveOrZeroInteger);
```
Here we created two variables: binary variable 'x' and non-negative integer 'y'. Now we can perform operations on these variables:
```
IVariable sumOfXAndY = x.Operation(OperationType.Addition, y);
IVariable anotherSumOfXAndY = solver.Operation(OperationType.Addition, x, y);
IVariable negationOfX = x.Operation(OperationType.Negation);
IVariable multiplicationOfXAndY = y.Operation(OperationType.Multiplication, x);
IVariable isYGreaterThanX = y.Operation(OperationType.IsGreaterThan, x);
```
We can also add constraints:
```
sumofXAndY.Set(ConstraintType.LessOrEqual, solver.FromConstant(3));
```
I hope you get the idea. You can chain operations using fluent interface.
Above code uses only functions from IMilpManager. If you want to solve the problem, you need to have implementation of a IMilpSolver. Fortunately, CPLEX package implements the latter interface so we are able to add goal and solve the problem:
```
solver.AddGoal("dummy_goal", sumOfXAndY);
solver.Solve();
Assert.That(solver.GetStatus(), Is.EqualTo(SolutionStatus.Optimal), "Model cannot be solved!");
double result = solver.GetValue(sumOfXAndY);
```
You can easily use different solvers, just replace implementation of IMilpManager (or IMilpSolver).

# Creating custom solver
If you want to integrate your own solver with this library, you need to implement IMilpManager or IMilpSolver, and create your custom IVariable. However, the easies way is to inherit from BaseMilpSolver and implement missing functions for creating variables, performing the most basic operations, and calculating the result. By doing this you can use all defined operators out of the box and don't care about the mathematical details, because your library just needs to take care of handling basic building blocks. See CPLEX implementation https://github.com/afish/CplexMilpSolver or Microsoft Solver Foundation implementation https://github.com/afish/MsfMilpSolver .
