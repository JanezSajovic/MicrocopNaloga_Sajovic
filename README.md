# Naloga Microcop 

## **Specifikacije**

#### 
Visual Studio 2019  
.Net Core 5.0  
Podatkovna baza - **Sqllite** - Tabeli Admins in Users
####

## **Testiranje**

####
Najprej je potrebno registrirati račun **admina**.  
Admin nam omogoča izvajanje vseh operacij nad **uporabniki** (ki niso admini).  
Nato se admin prijavi in ob prjavi prejme svoj **ApiKey** (JwtToken, zgeneriran na stran serverja).  
Tega nato uporabi za api klice nad uporabniki.  

![Admin in apikey](https://i.imgur.com/FmJR4KI.png)

Na prvi strani kliknemo gumb **Authorize** in v polje **Value** vnesemo
"**Bearer[presledek][admin apikey]**".  
Npr: "**Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE2MzY2Mjk1MjksImlzcyI6IlRl...**".  
Ob kliki na bumb **Authorize** potrdimo svoj **ApiKey** za validacijo vseh api klicev.  
Nato lahko izvajamo vse zaščitene api klice. 
####

## **Logiranje v .log datoteke**

####
Za logiranje sem uporabil knjižnico/paket **NLog**.  
Konfiguracija logiranja se nahaja v **nlog.config** datoteki znotraj projekta.  
Datoteke .log pa shranjujemo v mapo "**DailyLogFiles**" tudi znotraj projekta.  
Primer izpisa v .log datoteki

![Primer izpisa](https://i.imgur.com/HheXK5H.png)

####

## **Podatki v bazi**

### Admin baza - Za namene testiranja

| Uporabniško ime      | Geslo |
| -------------------- | ----------- |
| JakaAdmin      | Admin123!       |
| ToneAdmin   | SkrivnoGeslo!321!        |


### Uporabniki baza - Za namene testiranja

| ID | Uporabniško ime | Geslo | Celo ime | Email | Telefon | Jezik | Kultura | Validiran |
| - | - | - | - | - | - | - | - | - |
| 1 | CirilK | Ciril123! | Ciril Kosmač | ciril.k@gmail.com | 017541474 | slovenščina | si | true |
| 2 | ABoris | Boris456! | Boris A. Novak | novakov.boris@gmail.com | 030698747 | slovenščina | si | false |
| 4 | JKRowling | Harry789! | Joanne Rowling | Joanne.Rowling@gmail.com | 0802288 | angleščina | en | true |
| 5 | Preseren | Kam123!! | France Prešeren | povodni.moz@gmail.com | 040187968 | slovenščina | si | false|

####

Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJleHAiOjE2MzY3MjY5MTEsImlzcyI6IlRlc3QuY29tIiwiYXVkIjoiVGVzdC5jb20ifQ.bdqMkTtT2dIYodIvodT8REhL6iCb99HZHf4tRPmy8Ow