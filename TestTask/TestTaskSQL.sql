/*1. Если я правильно понимаю, что в DMZ таблицы NVM - это уникальный айдишник для каждого документа, то для связи DMZ С DMS не хватает в DMS поля ID таблицы DMZ (ndm - номер документа), 
которое бы характеризовало к какому документу относились бы продукты, указанные в DMS  */

/*2. Нарушения нормализации бд есть, в DMS еще зачем-то указан SORT, хотя он есть как поле в таблице товаров, на которую ссылкается и DMS через KTOV.
Также можно сделать еще одну таблицу - категория товаров (category_id, category_name), в которой указать, например (1, "Пиво"), а в таблице TOV потом ссылаться на нее через FK на category_id*/

/*3.1*/

/*в таблицу DMS добавили FK NDM, которое ссылается на DMZ*/

SELECT NTOV, SUM(KOL) AS "Количество (итого за день)", SUM(CENA * KOL) AS "Сумма (итого за день)" FROM DMZ 
JOIN DMS ON DMS.NDM = DMZ.NDM
JOIN TOV ON DMS.KTOV = TOV.KTOV
WHERE DDM = '01.05.2014' AND PR = 2
GROUP BY NTOV
ORDER BY SUM(CENA * KOL) DESC;

/*3.2*/

UPDATE DMS 
SET DMS.SORT = TOV.SORT FROM 
DMS JOIN TOV ON DMS.KTOV = TOV.KTOV; 

/*3.3*/

SELECT NTOV, SUM(CASE WHEN PR = 1 THEN KOL ELSE - KOL END) AS "Остаток", SUM(CASE WHEN PR = 1 THEN KOL * CENA ELSE - KOL * CENA END) AS "Сумма остатка" FROM DMZ 
JOIN DMS ON DMS.NDM = DMZ.NDM
JOIN TOV ON DMS.KTOV = TOV.KTOV
GROUP BY NTOV
ORDER BY NTOV;

/*3.4*/

INSERT INTO DMZ VALUES(
CAST(GETDATE() AS date), 
(CASE WHEN (SELECT COUNT(*) FROM DMZ) = 0 THEN 1 ELSE (SELECT TOP 1 NDM FROM DMZ ORDER BY NDM DESC) + 1 END), 
(CASE WHEN (SELECT COUNT(*) FROM DMZ WHERE PR = 1) > (SELECT COUNT(*) FROM DMZ WHERE PR = 2) THEN 2 ELSE 1 END)
);

/*3.5*/

DECLARE @MinNumber int;
SET @MinNumber = (SELECT TOP 1 NDM FROM DMZ ORDER BY NDM ASC);

DECLARE @MaxNumber int;
SET @MaxNumber = (SELECT TOP 1 NDM FROM DMZ ORDER BY NDM DESC);

SELECT * FROM DMS WHERE NDM = @MaxNumber


SELECT KTOV, @MaxNumber AS NDM, KOL, CENA, SORT INTO Temp FROM  DMZ 
JOIN DMS ON DMS.NDM = DMZ.NDM
WHERE DMS.NDM = @MinNumber;

INSERT INTO DMS (KTOV, NDM, KOL, CENA, SORT)
(SELECT * FROM Temp WHERE KTOV NOT IN (SELECT KTOV FROM DMS WHERE DMS.NDM = @MaxNumber));

SELECT * FROM DMS;

DROP Table Temp;