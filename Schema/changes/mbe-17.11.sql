SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='TRADITIONAL,ALLOW_INVALID_DATES';

CREATE TABLE time_clock (
	time_clock_id char(36) COLLATE utf8_unicode_ci NOT NULL,
	address varchar(16) COLLATE utf8_unicode_ci NOT NULL,
	last_sync datetime NOT NULL,
	sync tinyint(1) NOT NULL,
	PRIMARY KEY (time_clock_id)
) ENGINE=InnoDB;

CREATE TABLE time_clock_record (
	time_clock_record_id char(36) COLLATE utf8_unicode_ci NOT NULL,
	enroll_number int(11) NOT NULL,
	datetime datetime NOT NULL,
	type int(11) NOT NULL,
	employee int(11) DEFAULT NULL,
	PRIMARY KEY (time_clock_record_id),
	KEY time_clock_record_employee_idx (employee),
	CONSTRAINT time_clock_record_employee_fk
		FOREIGN KEY (employee) REFERENCES employee (employee_id)
		ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE=InnoDB;

ALTER TABLE inventory_receipt 
	DROP FOREIGN KEY inventory_receipt_employee_creator,
	DROP FOREIGN KEY inventory_receipt_employee_updater,
	DROP FOREIGN KEY inventory_receipt_purchase_order,
	DROP FOREIGN KEY inventory_receipt_warehouse,
	DROP INDEX inventory_receipt_employee_creator_idx,
	DROP INDEX inventory_receipt_employee_updater_idx;

ALTER TABLE inventory_receipt 
	ADD COLUMN store INT(11) NOT NULL AFTER inventory_receipt_id,
	ADD COLUMN serial INT(11) NULL AFTER store,
	ADD INDEX inventory_receipt_creator_idx (creator ASC),
	ADD INDEX inventory_receipt_updater_idx (updater ASC),
	ADD INDEX inventory_receipt_store_idx (store ASC),
	ADD CONSTRAINT inventory_receipt_creator_fk
		FOREIGN KEY (creator) REFERENCES employee (employee_id)
		ON DELETE NO ACTION ON UPDATE NO ACTION,
	ADD CONSTRAINT inventory_receipt_updater_fk
		FOREIGN KEY (updater) REFERENCES employee (employee_id)
		ON DELETE NO ACTION ON UPDATE NO ACTION,
	ADD CONSTRAINT inventory_receipt_warehouse_fk
		FOREIGN KEY (warehouse) REFERENCES warehouse (warehouse_id)
		ON DELETE NO ACTION ON UPDATE NO ACTION,
	ADD CONSTRAINT inventory_receipt_purchase_order_fk
		FOREIGN KEY (purchase_order) REFERENCES purchase_order (purchase_order_id)
		ON DELETE NO ACTION ON UPDATE NO ACTION,
	ADD CONSTRAINT inventory_receipt_store_fk
		FOREIGN KEY (store) REFERENCES store (store_id)
		ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE inventory_issue 
	DROP FOREIGN KEY inventory_issue_employee_creator,
	DROP FOREIGN KEY inventory_issue_employee_updater,
	DROP FOREIGN KEY inventory_issue_warehouse,
	DROP FOREIGN KEY inventory_issue_supplier_return_fk,
	DROP INDEX inventory_issue_supplier_return_fk_idx;

ALTER TABLE inventory_issue 
	ADD COLUMN store INT(11) NOT NULL AFTER inventory_issue_id,
	ADD COLUMN serial INT(11) NULL AFTER store,
	ADD INDEX inventory_issue_supplier_return_idx (supplier_return ASC),
	ADD INDEX inventory_issue_store_idx (store ASC),
	ADD CONSTRAINT inventory_issue_employee_creator_fk
		FOREIGN KEY (creator) REFERENCES employee (employee_id)
		ON DELETE NO ACTION ON UPDATE NO ACTION,
	ADD CONSTRAINT inventory_issue_employee_updater_fk
		FOREIGN KEY (updater) REFERENCES employee (employee_id)
		ON DELETE NO ACTION ON UPDATE NO ACTION,
	ADD CONSTRAINT inventory_issue_warehouse_fk
		FOREIGN KEY (warehouse) REFERENCES warehouse (warehouse_id)
		ON DELETE NO ACTION ON UPDATE NO ACTION,
	ADD CONSTRAINT inventory_issue_supplier_return_fk
		FOREIGN KEY (supplier_return) REFERENCES supplier_return (supplier_return_id)
		ON DELETE NO ACTION ON UPDATE NO ACTION,
	ADD CONSTRAINT inventory_issue_store_fk
		FOREIGN KEY (store) REFERENCES store (store_id)
		ON DELETE NO ACTION ON UPDATE NO ACTION;

ALTER TABLE inventory_transfer 
	DROP FOREIGN KEY inventory_transfer_employee_creator_fk,
	DROP FOREIGN KEY inventory_transfer_employee_updater_fk,
	DROP FOREIGN KEY inventory_transfer_warehouse_from_fk,
	DROP FOREIGN KEY inventory_transfer_warehouse_to_fk,
	DROP INDEX inventory_transfer_warehouse_from_fk_idx,
	DROP INDEX inventory_transfer_warehouse_to_fk_idx,
	DROP INDEX inventory_transfer_employee_creator_fk_idx,
	DROP INDEX inventory_transfer_employee_updater_fk_idx;

ALTER TABLE inventory_transfer 
	ADD COLUMN store INT(11) NOT NULL AFTER inventory_transfer_id,
	ADD COLUMN serial INT(11) NULL AFTER store,
	ADD INDEX inventory_transfer_from_idx (warehouse ASC),
	ADD INDEX inventory_transfer_to_idx (warehouse_to ASC),
	ADD INDEX inventory_transfer_creator_idx (creator ASC),
	ADD INDEX inventory_transfer_updater_idx (updater ASC),
	ADD INDEX inventory_transfer_store_idx (store ASC),
	ADD CONSTRAINT inventory_transfer_from_fk
		FOREIGN KEY (warehouse) REFERENCES warehouse (warehouse_id)
		ON DELETE NO ACTION ON UPDATE NO ACTION,
	ADD CONSTRAINT inventory_transfer_to_fk
		FOREIGN KEY (warehouse_to) REFERENCES warehouse (warehouse_id)
		ON DELETE NO ACTION ON UPDATE NO ACTION,
	ADD CONSTRAINT inventory_transfer_creator_fk
		FOREIGN KEY (creator) REFERENCES employee (employee_id)
		ON DELETE NO ACTION ON UPDATE NO ACTION,
	ADD CONSTRAINT inventory_transfer_updater_fk
		FOREIGN KEY (updater) REFERENCES employee (employee_id)
		ON DELETE NO ACTION ON UPDATE NO ACTION,
	ADD CONSTRAINT inventory_transfer_store_fk
		FOREIGN KEY (store) REFERENCES store (store_id)
		ON DELETE NO ACTION ON UPDATE NO ACTION;

UPDATE inventory_issue i,    warehouse w SET i.store = w.store WHERE w.warehouse_id = i.warehouse;
UPDATE inventory_receipt i,  warehouse w SET i.store = w.store WHERE w.warehouse_id = i.warehouse;
UPDATE inventory_transfer i, warehouse w SET i.store = w.store WHERE w.warehouse_id = i.warehouse;

SET @row_number:=0;
SET @store:=0;

UPDATE inventory_issue
SET serial=(@row_number := CASE WHEN @store=store THEN @row_number+1 ELSE 1 END),
	store = (@store := store)
ORDER BY store, inventory_issue_id;

SET @row_number:=0;
SET @store:=0;

UPDATE inventory_receipt
SET serial=(@row_number := CASE WHEN @store=store THEN @row_number+1 ELSE 1 END),
	store = (@store := store)
ORDER BY store, inventory_receipt_id;

SET @row_number:=0;
SET @store:=0;

UPDATE inventory_transfer
SET serial=(@row_number := CASE WHEN @store=store THEN @row_number+1 ELSE 1 END),
	store = (@store := store)
ORDER BY store, inventory_transfer_id;

ALTER TABLE inventory_receipt
	ADD UNIQUE INDEX inventory_receipt_store_serial_idx (store ASC, serial ASC);

ALTER TABLE inventory_issue
	ADD UNIQUE INDEX inventory_issue_store_serial_idx (store ASC, serial ASC);

ALTER TABLE inventory_transfer
	ADD UNIQUE INDEX inventory_transfer_store_serial_idx (store ASC, serial ASC);

ALTER TABLE sales_order
	ADD UNIQUE INDEX sales_order_store_serial_idx (store ASC, serial ASC);

UPDATE fiscal_document SET payment_method = payment_method + 100 WHERE payment_method > 1;
UPDATE fiscal_document SET payment_method = 99 WHERE payment_method = 107;
UPDATE fiscal_document SET payment_method = 00 WHERE payment_method = 106;
UPDATE fiscal_document SET payment_method = 03 WHERE payment_method = 105;
UPDATE fiscal_document SET payment_method = 02 WHERE payment_method = 104;
UPDATE fiscal_document SET payment_method = 28 WHERE payment_method = 103;
UPDATE fiscal_document SET payment_method = 04 WHERE payment_method = 102;

UPDATE customer_payment SET method = method + 100 WHERE method > 1;
UPDATE customer_payment SET method = 1001 WHERE method = 106;
UPDATE customer_payment SET method = 99 WHERE method = 107;
UPDATE customer_payment SET method = 03 WHERE method = 105;
UPDATE customer_payment SET method = 02 WHERE method = 104;
UPDATE customer_payment SET method = 28 WHERE method = 103;
UPDATE customer_payment SET method = 04 WHERE method = 102;

UPDATE supplier_payment SET method = method + 100 WHERE method > 1;
UPDATE supplier_payment SET method = 1001 WHERE method = 106;
UPDATE supplier_payment SET method = 99 WHERE method = 107;
UPDATE supplier_payment SET method = 03 WHERE method = 105;
UPDATE supplier_payment SET method = 02 WHERE method = 104;
UPDATE supplier_payment SET method = 28 WHERE method = 103;
UPDATE supplier_payment SET method = 04 WHERE method = 102;

ALTER TABLE sales_quote
	ADD COLUMN payment_terms TINYINT NOT NULL AFTER customer,
	ADD COLUMN creator INT(11) NOT NULL,
	ADD COLUMN updater INT(11) NOT NULL,
	ADD COLUMN creation_time DATETIME NULL,
	ADD COLUMN modification_time DATETIME NULL,
	ADD COLUMN contact INT(11) NULL,
	ADD COLUMN ship_to INT(11) NULL,
	ADD COLUMN comment VARCHAR(1024) NULL,
	ADD COLUMN currency INT NOT NULL,
	ADD COLUMN exchange_rate DECIMAL(8,4) NOT NULL DEFAULT 1,
	ADD INDEX sales_quote_creator_idx (creator ASC),
	ADD INDEX sales_quote_updater_idx (updater ASC),
	ADD INDEX sales_quote_ship_to_idx (ship_to ASC),
	ADD INDEX sales_quote_contact_idx (contact ASC),
	ADD CONSTRAINT sales_quote_creator_fk
		FOREIGN KEY (creator) REFERENCES employee (employee_id)
		ON DELETE NO ACTION ON UPDATE NO ACTION,
	ADD CONSTRAINT sales_quote_updater_fk
		FOREIGN KEY (updater) REFERENCES employee (employee_id)
		ON DELETE NO ACTION ON UPDATE NO ACTION,
	ADD CONSTRAINT sales_quote_ship_to_fk
		FOREIGN KEY (ship_to) REFERENCES address (address_id)
		ON DELETE NO ACTION ON UPDATE NO ACTION,
	ADD CONSTRAINT sales_quote_contact_fk
		FOREIGN KEY (contact) REFERENCES contact (contact_id)
		ON DELETE NO ACTION ON UPDATE NO ACTION;

UPDATE sales_quote
SET creation_time = date, modification_time = date, creator = salesperson, updater = salesperson;

ALTER TABLE sales_quote
	CHANGE COLUMN creation_time creation_time DATETIME NOT NULL,
	CHANGE COLUMN modification_time modification_time DATETIME NOT NULL;

ALTER TABLE sales_quote_detail
	ADD COLUMN comment VARCHAR(1024) NULL;

ALTER TABLE customer
	ADD COLUMN salesperson INT(11) NULL DEFAULT NULL;

-- TRUNCATE TABLE customer_payment;

ALTER TABLE customer_payment 
	DROP FOREIGN KEY customer_payment_sales_order,
	DROP INDEX customer_payment_sales_order_idx,
	DROP COLUMN sales_order,
	DROP COLUMN cash_change;

ALTER TABLE customer_payment
	ADD COLUMN currency INT(11) NOT NULL;

CREATE TABLE sales_order_payment (
	sales_order_payment_id int(11) NOT NULL AUTO_INCREMENT,
	sales_order INT(11) NOT NULL,
	customer_payment INT(11) NOT NULL,
	amount DECIMAL(18,4) NOT NULL,
	amount_change DECIMAL(18,4) NOT NULL,
	PRIMARY KEY (sales_order_payment_id),
	INDEX sales_order_payment_sales_order_idx (sales_order ASC),
	INDEX sales_order_payment_customer_payment_idx (customer_payment ASC),
	UNIQUE INDEX sales_order_payment_sales_order_customer_payment_idx (sales_order ASC, customer_payment ASC),
	CONSTRAINT sales_order_payment_sales_order_fk
		FOREIGN KEY (sales_order) REFERENCES sales_order (sales_order_id)
		ON DELETE NO ACTION ON UPDATE NO ACTION,
	CONSTRAINT sales_order_payment_customer_payment_fk
		FOREIGN KEY (customer_payment) REFERENCES customer_payment (customer_payment_id)
		ON DELETE NO ACTION ON UPDATE NO ACTION
) ENGINE = InnoDB;

ALTER TABLE employee
	ADD COLUMN enroll_number INT(11) NULL DEFAULT NULL;

ALTER TABLE sales_order
	ADD COLUMN customer_name VARCHAR(100) NULL DEFAULT NULL;
	
ALTER TABLE delivery_order
	ADD COLUMN delivered TINYINT(1) NULL DEFAULT 0;
	
CREATE TABLE `expenses` (
	`expense_id` INT(11) NOT NULL AUTO_INCREMENT,
	`expense` VARCHAR(100) NOT NULL DEFAULT '0' COLLATE 'utf8_unicode_ci',
	`comment` VARCHAR(500) NULL DEFAULT '0' COLLATE 'utf8_unicode_ci',
	PRIMARY KEY (`expense_id`)
);

CREATE TABLE `expense_voucher` (
	`expense_voucher_id` INT(11) NOT NULL AUTO_INCREMENT,
	`creator` INT(11) NOT NULL DEFAULT '0',
	`updater` INT(11) NOT NULL DEFAULT '0',
	`store` INT(11) NOT NULL DEFAULT '0',
	`cash_session` INT(11) NOT NULL DEFAULT '0',	
	`comment` VARCHAR(500) NULL DEFAULT NULL COLLATE 'utf8_unicode_ci',
	`date` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	`creation_time` DATETIME NOT NULL,
	`modification_time` DATETIME NOT NULL,
	`completed` TINYINT(1) NULL DEFAULT '0',
	`cancelled` TINYINT(1) NULL DEFAULT '0',
	PRIMARY KEY (`expense_voucher_id`),
	INDEX `expense_voucher_store_idx` (`store`),
	INDEX `expense_voucher_creator_idx` (`creator`),
	INDEX `expense_voucher_updater_idx` (`updater`),
	INDEX `expense_voucher_cash_session_idx` (`cash_session`),
	CONSTRAINT `expense_voucher_creator_fk` FOREIGN KEY (`creator`) REFERENCES `employee` (`employee_id`),
	CONSTRAINT `expense_voucher_updater_fk` FOREIGN KEY (`updater`) REFERENCES `employee` (`employee_id`),
	CONSTRAINT `expense_voucher_store_fk` FOREIGN KEY (`store`) REFERENCES `store` (`store_id`),
	CONSTRAINT `expense_voucher_cash_session_fk` FOREIGN KEY (`cash_session`) REFERENCES `cash_session` (`cash_session_id`)
);

CREATE TABLE `expense_voucher_detail` (
	`expense_voucher_detail_id` INT(11) NOT NULL AUTO_INCREMENT,
	`expense_voucher` INT(11) NOT NULL DEFAULT '0',
	`expense` INT(11) NOT NULL DEFAULT '0',
	`amount` DECIMAL(10,0) NOT NULL DEFAULT '0',
	`comment` VARCHAR(500) NULL DEFAULT NULL COLLATE 'utf8_unicode_ci',
	PRIMARY KEY (`expense_voucher_detail_id`),
	INDEX `expense_voucher_detail_expense_voucher_idx` (`expense_voucher`),
	INDEX `expense_voucher_detail_expenses_idx` (`expense`),
	CONSTRAINT `expense_voucher_detail_expense_voucher_fk`
		FOREIGN KEY (`expense_voucher`) REFERENCES `expense_voucher` (`expense_voucher_id`)
		ON UPDATE NO ACTION ON DELETE NO ACTION,
	CONSTRAINT `expense_voucher_detail_expenses_fk`
		FOREIGN KEY (`expense`) REFERENCES `expenses` (`expense_id`)
		ON UPDATE NO ACTION ON DELETE NO ACTION
);

ALTER TABLE customer_discount
	CHANGE COLUMN discount discount DECIMAL(9,8) NOT NULL;
	
ALTER TABLE customer_refund_detail
	CHANGE COLUMN discount discount DECIMAL(9,8) NOT NULL;
	
ALTER TABLE fiscal_document_detail
	CHANGE COLUMN discount discount DECIMAL(9,8) NOT NULL;
	
ALTER TABLE purchase_order_detail
	CHANGE COLUMN discount discount DECIMAL(9,8) NOT NULL;	

ALTER TABLE sales_order_detail
	CHANGE COLUMN discount discount DECIMAL(9,8) NOT NULL;

ALTER TABLE sales_quote_detail
	CHANGE COLUMN discount discount DECIMAL(9,8) NOT NULL;

ALTER TABLE supplier_return_detail
	CHANGE COLUMN discount discount DECIMAL(9,8) NOT NULL;	

CREATE TABLE `payment_method_option` (
	`payment_method_option_id` INT(11) NOT NULL AUTO_INCREMENT,
	`store` INT(11) NOT NULL,
	`name` VARCHAR(50) NOT NULL COLLATE 'utf8_unicode_ci',
	`number_of_payments` TINYINT(4) NOT NULL DEFAULT '1',
	`display_on_ticket` TINYINT(1) NOT NULL,
	`payment_method` INT(11) NOT NULL,
	`commission` DECIMAL(10,3) NOT NULL,
	`enabled` TINYINT(1) NOT NULL DEFAULT '1',
	PRIMARY KEY (`payment_method_option_id`),
	INDEX `payment_method_option_store_idx` (`store`),
	CONSTRAINT `payment_method_option_store_fk`
		FOREIGN KEY (`store`) REFERENCES `store` (`store_id`)
		ON UPDATE NO ACTION ON DELETE NO ACTION
);

ALTER TABLE `customer_payment`
	ADD COLUMN `commission` DECIMAL(10,4) NULL AFTER `method`,
	ADD COLUMN `payment_charge` INT(11) NULL AFTER `commission`,
	ADD CONSTRAINT `customer_payment_charge_fk` FOREIGN KEY (`payment_charge`)
		REFERENCES `payment_method_option` (`payment_method_option_id`)
		ON UPDATE NO ACTION ON DELETE NO ACTION;

CREATE TABLE `payment_on_delivery` (
	`payment_on_delivery_id` INT(11) NOT NULL AUTO_INCREMENT,
	`customer_payment` INT(11) NOT NULL,
	`cash_session` INT(11) NULL DEFAULT NULL,
	`paid` TINYINT(1) NOT NULL DEFAULT '0',
	`date` DATETIME NOT NULL DEFAULT CURRENT_TIMESTAMP,
	PRIMARY KEY (`payment_on_delivery_id`),
	INDEX `payment_on_delivery_customer_payment_idx` (`customer_payment`),
	INDEX `payment_on_delivery_cash_session_idx` (`cash_session`),
	CONSTRAINT `payments_on_deliveries_cash_session_fk`
		FOREIGN KEY (`cash_session`) REFERENCES `cash_session` (`cash_session_id`)
		ON UPDATE NO ACTION ON DELETE NO ACTION,
	CONSTRAINT `payments_on_deliveries_customer_payment_fk`
		FOREIGN KEY (`customer_payment`) REFERENCES `customer_payment` (`customer_payment_id`)
		ON UPDATE NO ACTION ON DELETE NO ACTION
);

ALTER TABLE `sales_order`
	ADD COLUMN `recipient` VARCHAR(13) NULL DEFAULT NULL AFTER `promise_date`,
	ADD COLUMN `recipient_name` VARCHAR(250) NULL DEFAULT NULL AFTER `recipient`,
	ADD COLUMN `recipient_address` INT NULL DEFAULT NULL AFTER `recipient_name`,
	ADD CONSTRAINT `FK_sales_order_address` FOREIGN KEY (`recipient_address`) REFERENCES `address` (`address_id`) ON UPDATE NO ACTION ON DELETE NO ACTION;


ALTER TABLE `customer_payment`
	ALTER `cash_session` DROP DEFAULT;
ALTER TABLE `customer_payment`
	CHANGE COLUMN `cash_session` `cash_session` INT(11) NULL AFTER `date`;
	
ALTER TABLE `delivery_order`
	ALTER `date` DROP DEFAULT;
ALTER TABLE `delivery_order`
	CHANGE COLUMN `date` `date` DATETIME NULL AFTER `ship_to`;

	
SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
