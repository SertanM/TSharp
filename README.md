# T#
It's a basic programming launguage written in C# to use games on unity (I haven't migrated it to Unity game engine yet.) I hope you will like it.

![Logo](Logo.png)
# Data Types

| Type            | C# Type           | Explanation       | Samples         |
|-----------------|-------------------|-------------------|-----------------|
| Number          | System.Int32      | It takes integers | 1, 2, 17, 852...|
| Bool            | System.Boolean    | It takes boleans  | true, false     |

# Binary Operators

| Operator        | Symbol            | Input Types     | Output Types      | Precedence |
|-----------------|-------------------|-----------------|-------------------|------------|
| Addition        | +                 | Number          | Number            | 4          |
| Subtraction     | -                 | Number          | Number            | 4          |
| Multiplication  | *                 | Number          | Number            | 5          |
| Division        | /                 | Number          | Number            | 5          |
| Power           | ^                 | Number          | Number            | 6          |
| Mod             | %                 | Number          | Number            | 5          |
| IsEqual         | ==                | Number, Bool    | Bool              | 3          |
| NotEqual        | !=                | Number, Bool    | Bool              | 3          |
| Bigger          | >                 | Number          | Bool              | 3          |
| Bigger Or Equal | >=                | Number          | Bool              | 3          |
| Smaller         | <                 | Number          | Bool              | 3          |
| Smaller Or Equal| <=                | Number          | Bool              | 3          |
| And             | &&                | Bool            | Bool              | 2          |
| Or              | \|\|              | Bool            | Bool              | 1          |

# Unary Operators

| Operator        | Symbol            | Input Types     | Output Types      | Precedence |
|-----------------|-------------------|-----------------|-------------------|------------|
| Addition        | +                 | Int             | Int               | 7          |
| Subtraction     | -                 | Int             | Int               | 7          |
| Not             | !                 | Bool            | Bool              | 7          |

# Variables

| Identifier Keyword | Type           | Explanation                                   |
|--------------------|----------------|-----------------------------------------------|
| var                | Bool, Number   | To assign a variable without specifying a type|
| let                | Bool, Number   | To assign a const without specifying a type   |

| Equals Symbol      | Explanation    | Example        |
|--------------------|----------------|----------------|
| =                  | To assign      | var = value    |

# Statements

| Keyword         | Example                                                               |
|-----------------|-----------------------------------------------------------------------|
| if              | if (condition) {<br>    statement <br>}                               |
| else            | if (condition) {<br>    statement <br>}<br>else {<br>statement<br>}   |
| for, to, with   | for (assignment) to (target) with (riseAmount){<br>    statement <br>}|
| while           | while (condition) {<br>    statement <br>}                            |
