Feature: TodoManagement
    As a user
    I want to be able to create, read, update, and delete TODO items

@area-todo
Scenario: Create Todo from nothing
    Given a todo with title 'Creation test'
    And the item 'implement hooks'
    And the item 'implement step binding'
    When the todo does not exist
    Then the todo is created with revision #1

@area-todo
Scenario: Deleting a todo removes the todo
    Given a todo with title 'Deletion test'
    And an id of 'd4f52100065345a89bf6d09656bbf159'
    When the todo was created
    Then the todo is removed
