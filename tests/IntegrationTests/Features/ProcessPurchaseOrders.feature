Feature: ProcessPurchaseOrders

Process a purchase order via an API endpoint

Scenario: Supplying the original business case
	Given I a purchase with the ID of "123" for customer "456" has the line items
	| line item                                |
	| Video "Comprehensive First Aid Training" |
	| Book "The G irl on the train"            |
	| Book Club Membership                     |
	When It is processed
	Then It produces a shipping slip
	| item                                     | quantity |
	| Video "Comprehensive First Aid Training" | 1        |
	| Book "The G irl on the train"            | 1        |
		And The customers current membership is "Book Club Membership"

Scenario: Supplying only physical products
	Given I a purchase with the ID of "123" for customer "456" has the line items
	| line item                                |
	| Video "Comprehensive First Aid Training" |
	| Book "The G irl on the train"            |
	When It is processed
	Then It produces a shipping slip
	| item                                     | quantity |
	| Video "Comprehensive First Aid Training" | 1        |
	| Book "The G irl on the train"            | 1        |
	
Scenario: Supplying only membership products
	Given I a purchase with the ID of "123" for customer "456" has the line items
	| line item                                |
	| Book Club Membership                     |
	When It is processed
	Then It produces no shipping slip
		And The customers current membership is "Book Club Membership"
	
Scenario: Supplying multiple membership products
	Given I a purchase with the ID of "123" for customer "456" has the line items
	| line item               |
	| Book Club Membership    |
	| Video Club Membership   |
	| Premium Club Membership |
	When It is processed
	Then It produces no shipping slip
		And The customers current membership is "Premium Club Membership"
	
Scenario: Supplying multiples of the same product
	Given I a purchase with the ID of "123" for customer "456" has the line items
	| line item                                |
	| Video "Comprehensive First Aid Training" |
	| Book "The G irl on the train"            |
	| Book "The G irl on the train"            |
	| Book "The G irl on the train"            |
	When It is processed
	Then It produces a shipping slip
	| item                                     | quantity |
	| Video "Comprehensive First Aid Training" | 1        |
	| Book "The G irl on the train"            | 3        |