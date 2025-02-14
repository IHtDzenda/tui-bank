# Projekt Banka 
- Autor: Jan Hradecký

## Druhy Účtů

|                                  | Běžný účet | Spořicí účet | Kreditní účet |
| :------------------------------- | :---------- | :----------- | :------------ |
| Přijímání a odesílání plateb     | ano          | ano           | ne            |
| Převod mezi účty                 | ano          | ano           | ne            |
| Úročení zůstatku                 | ne          | ano           | ne            |
| Možnost překročení zůstatku      | ne          | ne           | ano            |
| Limity výběru prostředků         | ne          | ano           | ano            |
| Dětský účet                      | ano          | ano           | ne            |

## Podrobné informace

### Přijímání a odesílání plateb

*   Posílání plateb mezi účty nespadající pod uživatele.

### Převod mezi účty

*   Převod mezi účty uživatele. Přesun z účtu A na účet B.

### Úročení zůstatku

*   Úroky se připisují na konci měsíce.
*   Sazbu určuje banka, nyní je 3 % p.a.
*   Vzorec pro výpočet: [https://moodle.ssps.cz/mod/book/view.php?id=6244&chapterid=776#yui\_3\_17\_2\_1\_1739560074919\_55](https://moodle.ssps.cz/mod/book/view.php?id=6244&chapterid=776#yui_3_17_2_1_1739560074919_55)

### Možnost překročení zůstatku

*   Klient může překročit zůstatek, pokud tak učiní, má 15 dní lhůtu na bezúročné splacení dluhu.
*   Pokud dluh není splacen, úročí se částka 15 % p.m.
*   Klient splácí dluh vkládáním peněz na účet (nejdříve úroky, poté jistinu).
*   Nejdříve se splácí úvěr, pak až jistina.

### Limity výběru prostředků

*   Klient nebude schopen vybrat prostředky, které nejsou na účtu.
*   U spořícího účtu je limit 10 000 Kč za den a 50 000 Kč za týden.
*   U kreditního, neboli úvěrového účtu, nelze vybrat peníze.

### Dětský účet

*   Účet si může vytvořit osoba mladší 18 let.

## Druhy oprávnění

|                                               | Uživatel | Bankéř | Admin |
| :-------------------------------------------- | :------- | :------- | :------- |
| Manipulování s vlastními prostředky           | ano       | ne       | ne       |
| Manipulování s prostředky banky               | ne       | ano       | ne       |
| Přehled o oběhu peněz v bance                 | ne       | ano       | ano       |
| Správa uživatelských účtů a oprávnění         | ne       | ne       | ano       |

### Klienti

*   Přístup pouze ke svým účtům, možnost sledovat transakce a úroky.

### Bankéři

*   Přístup ke všem účtům, přehled o celkovém objemu vkladů/úroků.

### Administrátoři

*   Správa uživatelských účtů a oprávnění.

### Popis

#### Uživatel bude moct

*   Vybrat prostředky, pokud budou splněny všechny podmínky.
*   Poslat platbu, pokud budou splněny všechny podmínky.
*   Převádět peníze mezi účty, pokud budou splněny všechny podmínky.
*   Vytvořit účet, pokud budou splněny všechny podmínky.

#### Bankéř bude moct

*   Zobrazit účty ostatních uživatelů.
*   Zobrazit zůstatek a dluhy na daných účtech.
*   Zobrazit kapitál banky.

#### Administrátor bude moct

*   Vytvořit uživatele.
*   Vytvořit bankéře.


## Technologie
* C#
* SQLite
* Knihovny:
    * [Efcore](https://github.com/dotnet/efcore) jako ORM
    * [SpecterConsole](https://github.com/spectreconsole/spectre.console) pro lepši TUI

