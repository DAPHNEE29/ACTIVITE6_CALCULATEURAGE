# Activité 6 — Calculateur d'âge (.NET MAUI, MVVM)

Application de calcul d'âge réalisée en **.NET MAUI (C# / XAML)** dans le cadre
de l'atelier de développement mobile. Le sujet demande de réécrire en MVVM une
application « code-behind », puis **d'enrichir le projet avec une ou plusieurs
fonctionnalités supplémentaires, sans écrire la moindre ligne de logique dans le
code-behind**.

---

## 1. Règle finale du sujet

> Si un ViewModel contient le mot `Label`, `Entry`, `Button` ou `DisplayAlert`,
> ce n'est pas du MVVM.

Cette règle est appliquée ici sans exception :

| Fichier | Contenu |
| --- | --- |
| `MainPage.xaml.cs` | 1 constructeur : `InitializeComponent()` + `BindingContext` |
| `Views/ResultatPage.xaml.cs` | 1 constructeur + transmission des paramètres + `OnAppearing` |
| `AppShell.xaml.cs` | 1 enregistrement de route |
| `ViewModels/*` | **tout** l'état et **toutes** les actions |

Aucun `x:Name`, aucun `Clicked`, aucun `DisplayAlert` dans les pages.

---

## 2. Fonctionnalités

### Socle du TP

- Saisie du **nom** (`Entry` en `TwoWay`) et de la **date de naissance**
  (`DatePicker`).
- **Calcul** de l'âge en années pleines : si l'anniversaire n'est pas encore
  passé cette année, on retire une année.
- **Navigation** vers une page de détail par le **routing du Shell**, en
  transmettant le nom et l'âge dans l'URL (`?nom=…&age=…`) ; les paramètres sont
  reçus par `[QueryProperty]`.
- Bouton **Calculer** automatiquement **grisé** tant que le nom est vide.

### Fonctionnalités ajoutées pour l'activité 6

1. **Commande « Message » → propriété `Message` → `Label`.**
   Une commande écrit dans la propriété `Message`, et un simple `Label` lié à
   cette propriété l'affiche. Le message est *contextuel* : avant un calcul il
   indique quoi faire, après un calcul il rappelle le nom, la date de naissance
   et l'âge obtenu. Aucune ligne d'interface dans le ViewModel.

2. **Bouton « Réinitialiser ».**
   Remet **tous** les champs à zéro : nom vide, date par défaut, âge à 0,
   résultat et message vidés et masqués, retour à l'étape 1. Le bouton se
   grise à nouveau de lui-même une fois la remise à zéro faite.

3. **Bouton « Suivant » qui affiche les champs restants.**
   Écrit dans la propriété `ChampsRestants` le nom des champs qui manquent
   encore, puis n'atteint l'étape suivante — la page de résultat, atteinte par
   le routing — que lorsque plus rien ne manque :

   | Situation | `ChampsRestants` | Étape atteinte |
   | --- | --- | --- |
   | formulaire vide | `Champs restants : Nom, Date de naissance.` | aucune (étape 1) |
   | nom seul saisi | `Champs restants : Date de naissance.` | aucune (étape 1) |
   | formulaire complet | `Tous les champs sont renseignés.` | étape 2, page de résultat |

   Un Label `Progression` affiche en permanence l'étape courante
   (« Étape 1 sur 2 — Saisie », puis « Étape 2 sur 2 — Résultat »).

   Cette fonctionnalité apporte aussi une **validation** : une date de naissance
   située aujourd'hui ou dans le futur compte comme champ non renseigné, et
   `CalculerCommand` se grise tant que le formulaire est incomplet.

4. **Navigation abstraite (`INavigationService`).**
   Le ViewModel doit pouvoir navigate sans connaître `Shell`. Un petit service
   est donc injecté : `ShellNavigationService` est le **seul** point du
   programme qui touche à `Shell.Current`. Le ViewModel devient testable sans
   interface graphique.

5. **`AsyncRelayCommand`.**
   `Shell.GoToAsync` retourne une `Task` qu'un `Action` ne peut pas attendre.
   Cette variante asynchrone encapsule le `async void` imposé par `ICommand`
   dans un `try/catch`, ce qu'un `async void` écrit directement dans le
   code-behind ne serait pas.

6. **Bindings compilés (`x:DataType`).**
   Chaque page déclare le type de son ViewModel : une propriété mal orthographiée
   dans un `{Binding}` devient une **erreur de compilation** au lieu d'échouer
   silencieusement à l'exécution.

---

## 3. Organisation du projet

```
Activite6_MAUI_CalculateurAge/
├── CalculateurAge.sln
├── .gitignore
└── src/
    └── CalculateurAge/
        ├── ViewModels/
        │   ├── BaseViewModel.cs          INotifyPropertyChanged + SetField
        │   ├── RelayCommand.cs           Action -> ICommand
        │   ├── AsyncRelayCommand.cs      Func<Task> -> ICommand
        │   ├── CalculateurViewModel.cs   état + actions de la page principale
        │   └── ResultatPageViewModel.cs  paramètres reçus par le routing
        ├── Services/
        │   ├── INavigationService.cs
        │   └── ShellNavigationService.cs seul code qui touche à Shell
        ├── Views/
        │   ├── ResultatPage.xaml
        │   └── ResultatPage.xaml.cs
        ├── MainPage.xaml(.cs)
        ├── AppShell.xaml(.cs)            enregistrement de la route
        ├── MauiProgram.cs                injection de dépendances
        └── Platforms/                   .Android, iOS, MacCatalyst, Windows
```

---

## 4. Le trajet d'une donnée

1. L'utilisateur tape une lettre dans l'`Entry`.
2. Le binding, en `TwoWay`, écrit dans la propriété `Nom` du ViewModel.
3. Le setter détecte un changement et appelle `SetField`.
4. `SetField` affecte le champ privé puis lève `PropertyChanged("Nom")`.
5. Tous les contrôles liés à `Nom` sont notifiés et se redessinent.

> La vue connaît le ViewModel. Le ViewModel ignore la vue. Et pourtant l'écran se
> met à jour : c'est l'événement qui fait le lien.

---

## 5. Compilation et exécution

```bash
# Android (cible de l'énoncé)
dotnet build src/CalculateurAge/CalculateurAge.csproj -f net9.0-android

# Windows
dotnet build src/CalculateurAge/CalculateurAge.csproj -f net9.0-windows10.0.19041.0
```

Les deux cibles compilent avec **0 erreur et 0 avertissement**.

```bash
# Lancer sur l'émulateur Android
dotnet build src/CalculateurAge/CalculateurAge.csproj -f net9.0-android -t:Run
```

> **Fermer l'application avant de recompiler la cible Windows.** Windows
> verrouille un exécutable en cours d'exécution : si `CalculateurAge.exe` est
> encore lancé, la copie de sortie échoue avec `MSB3027` / `MSB3021`, alors que
> la compilation a bien réussi. C'est un problème de copie, pas de code.
>
> ```bash
> taskkill /IM CalculateurAge.exe /F
> dotnet build src/CalculateurAge/CalculateurAge.csproj -f net9.0-windows10.0.19041.0
> ```
>
> La cible Android n'est pas concernée.

---

## 6. Scénario de démonstration (vidéo de 30 s)

1. L'écran s'ouvre sur « Étape 1 sur 2 — Saisie », **Calculer** est grisé.
2. On tape un nom → on appuie sur **Suivant** → « Champs restants : Date de
   naissance. »
3. On choisit une date de naissance, on appuie sur **Calculer** →
   « *Daphnée, vous avez 29 ans* » apparaît.
4. On appuie sur **Suivant** → « Tous les champs sont renseignés. », l'étape
   passe à « Étape 2 sur 2 — Résultat » et la page de résultat s'ouvre avec le
   nom et l'âge reçus par le routing. **Retour** ramène à l'accueil.
5. On appuie sur **Message** → le message contextuel s'affiche.
6. On appuie sur **Réinitialiser** → tout revient à l'étape 1 et les boutons
   se grisent.