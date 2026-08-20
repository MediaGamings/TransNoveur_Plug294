![GitHub all releases](https://img.shields.io/github/downloads/MediaGamings/TransNoveur_Plug294/total)

# TransNoveur_Plug294

Plugin d'entreprise pour **Nova-Life**, pensé pour le RP autour de la restauration : les membres de l'entreprise approvisionnent les commerces de la ville en véhicules, ingrédients, machines et marchandises illégales.

## Fonctionnalités

Menu principal accessible par la touche `=` (membres de l'entreprise) ou via un point d'interaction placé en ville.

### 🚗 Voiture
Concessionnaire de 40 véhicules en 5 catégories, avec confirmation avant chaque achat. Les véhicules rejoignent un **garage personnel** (point d'interaction dédié) et ressortent dans la zone de spawn configurée. La catégorie **Mécanique** fournit bougie, courroie, batterie, batterie portable et bidons d'essence (unité et gros formats) à 50 % des prix officiels.

### 🥖 Nourriture — « Fournisseur Restauration »
38 ingrédients en 5 catégories : Station Exo, Boulangerie, Fast Food, Fruits & Graines, Café. Uniquement des denrées non craftables, en gros format lorsqu'il existe (cartons, cagettes, sacs, piles). Prix : 50 % des prix officiels — l'entreprise garde une marge de revente.

### ⚙️ Machines
Toutes les machines du jeu, rangées par métier en 9 catégories : **Bois**, **Métallurgie**, **Plastique & Carton**, **Électronique**, **Production alimentaire**, Cuisine & Fast Food, Café, Tireuses et Ustensiles. Des machines à découper le bois aux hauts fourneaux, en passant par la production de plastique, de carton et de PCB, le robot boulangerie et la machine à vin — sans oublier le four de boulangerie, l'établi fast-food, la friteuse, les machines à café (dont les 6 coloris capsules) et les tireuses. Prix : **50 % des prix officiels** sur les machines ; les ustensiles de démarrage gardent leurs prix officiels.

### 🔫 Illégal — « Fournisseur Illégal »
Outils de l'IllegalPoint (pied de biche, tenaille, couteaux), armes et composants complets de la culture de cannabis, pour approvisionner gangs et organisations. Prix officiels.

### 🛠️ Configuration (admins niveau 4 minimum)
Tout se configure **en jeu** : points garage, menu et spawn des véhicules, entreprise (ID), webhook Discord. Sauvegarde automatique dans `config.json`.

## 💳 Paiement bancaire automatique
Sans porte-monnaie ou avec un solde insuffisant en poche, la somme est prélevée sur le compte en banque, avec notification. Actif sur tous les achats : concessionnaire et les trois shops.

## 📜 Logs Discord
Chaque transaction (véhicule, shop, garage), prélèvement bancaire, action admin et erreur du plugin est envoyée en embed dans un salon Discord : le serveur dispose de preuves en cas de litige joueur. **Chaque menu a sa couleur de cadre** — bleu pour la voiture, vert pour la restauration, gris pour les machines, violet pour l'illégal — et l'embed d'achat précise la catégorie de l'article. Le webhook se configure **en jeu**, une seule fois, puis est verrouillé — modifiable uniquement dans `config.json`, l'URL n'est jamais réaffichée en jeu.

## Installation

1. Télécharger `TransNoveur_Plug294.dll` depuis la [dernière release](../../releases/latest).
2. La copier dans le dossier `Plugins` du serveur.
3. Redémarrer le serveur.
4. En jeu, avec un compte admin : menu → **Configuration** pour placer les points et définir l'ID de l'entreprise.

## Compatibilité

À jour des dernières mises à jour de Nova-Life : système d'argent par portes-monnaie, nouvelles icônes, noms d'items localisés. Tous les ID d'items et de véhicules sont vérifiés contre la table officielle d'Amboise.

---

*Développé par MediaGamings.*
