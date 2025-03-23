# Projekt Banka - ( Funkční Specifikace )

- Autor: Jan Hradecký

## Přehled
Tento dokument definuje funkční specifikaci pro bankovní systém, který podporuje různé typy účtu a operace, včetně správy oprávnění. Jedná se o TUI aplikaci (Text-based User Interface).


## Funkčnost

### Druhy účtů a jejich vlastnosti
|                                  | Běžný účet | Spořicí účet | Kreditní účet |
| :------------------------------- | :---------- | :----------- | :------------ |
| Přijímání a odesílání plateb     | Ano          | Ano           | Ano           |
| Převod mezi účty                 | Ano          | Ano           | Ano           |
| Úrčení zůstatku                 | Ne          | Ano           | Ne            |
| Možnost překročení zůstatku      | Ne          | Ne           | Ano           |
| Limity výběru prostředků         | Ne          | Ano           | Ano           |
| Dětský účet                      | Ano          | Ano           | Ne            |

###  Operace nad účty
#### Přijímání a odesílání plateb
- Umožnuje platby mezi různými účty.

#### Převod mezi účty
- Umožnuje převod mezi vlastními účty uživatele.

#### Úrčení zůstatku
- Spořicí účet umožňuje přípis úroků na konci měsíce (aktuální sazba 3 % p.a.).

#### Možnost překročení zůstatku
- Kreditní účet umožňuje překročení zůstatku.
- Bezúročné splacení možné do 15 dní.
- Po 15 dnech se uplatňuje úkor 15 % p.m.

#### Limity výběru prostředků
- Spořicí účet: Denní limit 10 000 Kč, týdenní limit 50 000 Kč.
- Kreditní účet: Nelze vybírat peníze.

#### Dětský účet
- Může si založit osoba mladší 18 let.


## Role a oprávnění

|                                               | Uživatel | Bankéř | Admin |
| :-------------------------------------------- | :------- | :------- | :------- |
| Manipulování s vlastními prostředky           | Ano       | Ne       | Ne       |
| Manipulování s prostředky banky               | Ne       | Ano       | Ne       |
| Přehled o oběhu peněz v bance                 | Ne       | Ano       | Ano       |
| Správa uživatelských účtů a oprávnění         | Ne       | Ne       | Ano       |


## Technická Realizace

### Uživatelské Rozhraní - přihlášení
Pomocí šipek / hesla a jména se uživatel bude moc přihlásit.

### Uživatelské Rozhraní
- Menu navigace s možnostmi: 
  - [1] Správa účtu
  - [2] Převod mezi účty
  - [3] Historie transakcí
  - [4] Odhlásit se
- Nebo navigace pomocí TAB a SHIFT+TAB
- Barevné odlišení chyb a úspěšných operací

### Řešení Chybových Stavů
- Nedostatek peněz: "Chyba: Nedostatečný zůstatek na účtu."
- Neplatná částka: "Chyba: Zadaná částka musí být kladná."
- Neplatné přihlašovací údaje: "Chyba: Nesprávné uživatelské jméno nebo heslo."
- Překročení deního limitu: "Chyba: Nelze vybrat více než 10 000 Kč denně."


## Technologie
- **Programovací jazyk:** C#
- **Databáze:** SQLite
- **Použité knihovny:**
    - [Efcore](https://github.com/dotnet/efcore) pro ORM.
    - [SpecterConsole](https://github.com/spectreconsole/spectre.console) pro TUI.



