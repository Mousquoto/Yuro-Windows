# YURO pour Windows

Application officielle de YURO : https://yurocollection.fr. Internet est nécessaire. La connexion Discord et la progression du site restent synchronisées.

## Distribution

Les installateurs et leurs métadonnées sont publics dans https://github.com/Mousquoto/Yuro-Windows/releases. Le dépôt du jeu Mousquoto/Yuro reste privé. Ce dépôt public contient seulement l’enveloppe Windows, son installateur et le workflow de construction.

## Construire et publier

Depuis desktop : npm ci puis npm run dist. Le script build-installer.ps1 construit un installateur .NET x64 par utilisateur et le fichier latest.yml dans le dossier YURO-PC voisin de la racine du dépôt. Il crée les raccourcis Bureau/Démarrer et une entrée de désinstallation Windows. Il accepte /S, --updated et --force-run pour les mises à jour silencieuses. Les téléchargements sont complets, sans blockmap.

Pour une mise à jour de l’application, augmenter la version dans package.json et package-lock.json, synchroniser desktop/ et .github/workflows/desktop-windows.yml dans le dépôt public puis pousser sur main. Le workflow Windows construit et publie une Release vX.Y.Z avec l’installateur versionné, l’alias YURO-Installation.exe et latest.yml. Il refuse de remplacer une version existante. Vérifier la réussite du workflow et le téléchargement public.

## Mises à jour

Les nouveautés du site apparaissent à la prochaine ouverture après son déploiement. Aucune actualisation forcée n’interrompt un combat. Une nouvelle version Windows est téléchargée en arrière-plan, annoncée au joueur puis installée à la fermeture. Les anciennes versions locales configurées sur le dépôt privé doivent être remplacées une première fois par cet installateur public.

L’installateur ne possède pas de certificat de signature numérique ; Windows peut afficher « éditeur inconnu » ou un avertissement SmartScreen.

## Mode jeu

Plein écran sans barre de titre. F11 alterne le plein écran, Alt+F4 quitte, F5 ou Ctrl+R actualise, Ctrl+Maj+U recherche une mise à jour.
