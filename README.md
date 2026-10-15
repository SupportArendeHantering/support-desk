Grupparbete av: *Ali, Erik och Anton.*
---

# Support Desk

Support Desk är ett ärendehanteringssystem byggt i WinUI för att koppla support tickets till kunder och handledare. Detta för att underlätta för företagets support avdelning och veta hur de ska planera sitt arbete utifrån nya, pågående och avklarade ärenden.

## Använd programmet

För att köra programmet:
- Klona eller ladda ner develop branchen.
- Sätt SupportDesk.Presentation som startup project.
- Kör projektet.

---
## Projektets struktur

| Proejt | Ansvarsområde |
|:-----|:------|
| Domain | I Domain projektet har vi samlat projektets modeller [*Customer, SupportTicket, TicketStatus* etc.] samt Value objects som modellerna använder [***Email, TicketDescription, CommentText*** etc.]  |
| Application  | I Application projektet så har vi våra Services och Interfaces [***CommentService, ITicketService, TicketService*** etc.] |
| Infrastructure | Infrastucture består av våra repositories samt datamodeller för den data som sparas i våra repositories [***JsonCustomerRepository, TicketData*** etc.]  |
| Presentation  | Presentationlagret är ett WinUi projekt som innehåller vår Nagigering, ViewModels och Pages [***CustomerPage, NavigationService*** etc.]  |
| Test  | Xunit projekt som innehåller tester för Domain, Application och Infrastructure projektet  |

### Beroendetabell mellan projekten

```mermaid
graph TD

    Domain[Domain]:::core

    Application[Application]:::layer
    Infrastructure[Infrastructure]:::layer
    Presentation[Presentation]:::layer

    %% Dependencies
    Application --> Domain
    Infrastructure --> Application
    Infrastructure --> Domain
    Presentation --> Application
    Presentation --> Infrastructure
    Presentation --> Domain

    classDef core fill:#f2f2f2,stroke:#333,stroke-width:1px,color:#000;
    classDef layer fill:#ffffff,stroke:#666,stroke-width:1px,color:#000;
```

## Datalagring

Datan för kunder, support-tickets och kommentarer sparas i .json filer.
filerna sparas i:  
`AppData\Local\Packages\[PFN]\LocalCache\Local\SupportDesk`  
där PFN (Package Family Name) som är ett unikt ID för installationen av applikationen och företaget som publicerat den.

## Arbetsfördelning
Då vi valde att bryta ner arbetet till mindre delar så har vi alla varit lite delaktiga i det mesta, men nedan följer en lite mer ingående uppdelning på fokusområden.

### Ali

### Erik

### Anton
#### Backend
- Value Objects för Customer 
- JSON settingsfil
- Tilldela Customer till SupportTicket
- Tilldela Handledare till en SupportTicket
- Filtrera Tickets efter status
- Lagt till tester enhet och integration

#### Frontend
- Customer vyn
- InfoBar som visar olika typer av info och felmeddelanden
- Lagt till så att kunddropdown blir inaktiv när inga kunder finns inlaggda.


## Resultat av manuell kontroll

## Ai användning i projektet
