﻿/*Creación de tabla*/
--DROP TABLE PRODUCT
CREATE TABLE PRODUCT(
ID NUMBER NOT NULL,
DESCRIPTION VARCHAR2(100),
PRICE NUMBER,
CATEGORY_ID NUMBER);

--DROP TABLE CATEGORY
CREATE TABLE CATEGORY(
ID NUMBER NOT NULL,
DESCRIPTION VARCHAR2(100));

ALTER TABLE PRODUCT ADD CONSTRAINT PK_PRODUCT PRIMARY KEY(ID);
ALTER TABLE CATEGORY ADD CONSTRAINT PK_CATEGORY PRIMARY KEY(ID);
ALTER TABLE PRODUCT ADD CONSTRAINT FK_PRODUCT_CATEGORY FOREIGN KEY (CATEGORY_ID) REFERENCES CATEGORY(ID);

/*Creación de sequencias*/
--drop sequence SEQ_PRODUCT;
CREATE SEQUENCE SEQ_PRODUCT MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 1;
--drop sequence SEQ_CATEGORY;
CREATE SEQUENCE SEQ_CATEGORY MINVALUE 1 MAXVALUE 9999999999999999999999999999 INCREMENT BY 1 START WITH 1;

/*Creación del UDT*/
--DROP TYPE PRODUCT_UDT FORCE;
CREATE OR REPLACE TYPE PRODUCT_UDT AS OBJECT(
ID NUMBER(20),
DESCRIPTION VARCHAR2(100),
PRICE NUMBER(20)) NOT FINAL INSTANTIABLE;

/*Creación de la tabla UDT*/
CREATE OR REPLACE TYPE PRODUCT_TABLE_UDT AS TABLE OF PRODUCT_UDT;

/*Firma SP*/
CREATE OR REPLACE PACKAGE PKG_TEST_01 AS
  --TYPE PRODUCT_TABLE_UDT IS TABLE OF PRODUCT%ROWTYPE;--Sólo funciona en PL-SQL sin embargo si quisiera pasar valores a este tipo desde C#, es imposible.
  
  PROCEDURE CREATE_CATEGORY(
    P_DESCRIPTION   IN CATEGORY.DESCRIPTION%TYPE,
    P_PRODUCTS      IN PRODUCT_TABLE_UDT,
    P_ID            OUT CATEGORY.ID%TYPE
  );
END PKG_TEST_01;

/*Cuerpo SP*/
CREATE OR REPLACE PACKAGE BODY PKG_TEST_01 AS 
  PROCEDURE CREATE_CATEGORY(
    P_DESCRIPTION   IN CATEGORY.DESCRIPTION%TYPE,
    P_PRODUCTS      IN PRODUCT_TABLE_UDT,
    P_ID            OUT CATEGORY.ID%TYPE
  )AS
  BEGIN
    INSERT INTO CATEGORY(ID,DESCRIPTION) VALUES(SEQ_CATEGORY.NEXTVAL,P_DESCRIPTION) RETURNING ID INTO P_ID;
    
    IF(P_PRODUCTS.COUNT>0 AND P_ID>0)THEN      
      FOR i IN P_PRODUCTS.FIRST .. P_PRODUCTS.LAST
      LOOP
        INSERT INTO PRODUCT(ID,DESCRIPTION,PRICE,CATEGORY_ID) 
        VALUES(SEQ_PRODUCT.NEXTVAL,P_PRODUCTS(i).DESCRIPTION,P_PRODUCTS(i).PRICE,P_ID);
      END LOOP;
    END IF;
  END;
END PKG_TEST_01;

SELECT * FROM CATEGORY;
SELECT * FROM PRODUCT;

DELETE FROM PRODUCT;
DELETE FROM CATEGORY;
COMMIT;