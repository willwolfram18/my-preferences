# TODO DIAGRAM NAME
> Replace this blockquote with a description of the diagram

# Primary Representation

```mermaid
sequenceDiagram
    actor Alice
    actor Bob
    actor Clare
    actor Billy [[./billy.svg]]

    Alice -> Bob: Tell Clare "Hello!"

    activate Bob
    Bob -> Clare: Alice says "Hello!"

    activate Clare
    Clare --> Bob: Hello, Alice!
    deactivate Clare

    Bob --> Alice: Clare says "Hello, Alice!"
    deactivate Bob

    note over Alice: wait 10 mins

    Alice ->(20) Clare: Texts "Hi again!"
```
# Element Catalog
<!-- Fill out the below sections with any relevant information or N/A -->

## Elements and their properties
|Element Name|Description|Properties|Notes|
|------------|-----------|----------|-----|
| Alice | An example actor in the exchange of information. | <ul> <li>Age: 20 years old</li> </ul> | N/A |
| Bob | The intermediary for communicating between Alice and Clare. | <ul> <li>Age: 21 years old</li> </ul> | N/A
| Clare | TODO | TODO | TODO |

## Element Interfaces
TODO(?) - probably links to Swagger/GraphQL/AsyncAPI specs?

## Element Behavior
Maybe?

# Context Diagram
<!-- Probably want to create a reusable context diagram that can be pulled in here -->
<!-- ![Context diagram](./context-diagram.puml) -->

# Variability Guide

# Design Rationale
TODO

# Related Views
TODO
