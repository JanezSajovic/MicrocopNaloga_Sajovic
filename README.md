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