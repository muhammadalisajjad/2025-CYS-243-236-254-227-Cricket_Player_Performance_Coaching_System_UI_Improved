# UI Design Notes for Pencil Tool

These are text wireframes. You can copy these into Pencil Project and draw proper UI diagrams.

## 1. Login Form

```text
+---------------------------------------------------+
| Cricket Player Performance System                 |
+---------------------------------------------------+
| Username: [____________________]                  |
| Password: [____________________]                  |
|                                                   |
| [ Login ]                         [ Exit ]        |
+---------------------------------------------------+
```

Controls used: text box, password field, buttons, panel.

## 2. Dashboard Form

```text
+---------------------------------------------------+
| File | Logout | Exit                              |
+---------------------+-----------------------------+
| Left Menu Panel     | Main Content Panel           |
| [Players]           | Welcome Coach/Admin          |
| [Coaches]           | System Statistics            |
| [Teams]             | Total Players                |
| [Training]          | Total Teams                  |
| [Fitness]           | Recent Matches               |
| [Matches]           |                             |
| [Reports]           |                             |
+---------------------+-----------------------------+
```

Controls used: menu strip, panels, buttons, labels.

## 3. Add/Edit Player Form

```text
+---------------------------------------------------+
| File | Save | Clear | Back                        |
+---------------------------------------------------+
| Player Basic Information                          |
| Full Name:       [____________________]           |
| Email:           [____________________]           |
| Phone:           [____________________]           |
| DOB:             [ date selector      ]           |
| Gender:          ( ) Male  ( ) Female             |
| Role:            [ dropdown          v]           |
| Batting Style:   [ dropdown          v]           |
| Bowling Style:   [ dropdown          v]           |
| Active:          [x]                              |
| Notes:           [ multi-line text area ]         |
|                                                   |
| [ Save Player ] [ Update Player ] [ Clear ]       |
|                                                   |
| Players Table                                    |
| ------------------------------------------------ |
| Name | Role | Team | Status | [Edit] | [Delete]   |
+---------------------------------------------------+
```

Controls used: text boxes, dropdowns, radio buttons, checkbox, date selector, text area, scrollbar, table, buttons inside table, panels.

## 4. Training and Attendance Form

```text
+---------------------------------------------------+
| File | Save | Back                                |
+---------------------------------------------------+
| Session Title: [____________________]             |
| Coach:         [ dropdown v ]                     |
| Team:          [ dropdown v ]                     |
| Date:          [ date selector ]                  |
| Focus Area:    [ dropdown v ]                     |
| Notes:         [ text area ]                      |
|                                                   |
| Attendance Table                                 |
| Player | Present | Late | Absent | Remarks         |
| Ali    |   o     |  o   |   o    | [_____]         |
+---------------------------------------------------+
```

## 5. Reports Form

```text
+---------------------------------------------------+
| File | Generate | Back                            |
+---------------------------------------------------+
| Report Type: [ dropdown v ]                       |
| Player:      [ dropdown v ]                       |
| Team:        [ dropdown v ]                       |
| Date From:   [ date selector ]                    |
| Date To:     [ date selector ]                    |
| Output Path: [____________________] [Browse]      |
|                                                   |
| [ Generate PDF ]                                  |
|                                                   |
| Report Preview Table                              |
+---------------------------------------------------+
```

Controls used: dropdowns, date selectors, text boxes, buttons, panel, table.
