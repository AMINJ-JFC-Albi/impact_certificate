-> dialogue_debut


=== dialogue_debut ===
Bonjour Reine! Comment vas-tu aujourd'hui? # speaker: Alien # delay:5

Salut Alien! Je vais très bien, merci! Et toi? # speaker: Reine  

Moi aussi! Tu as vu le nouveau projet? # speaker: Alien


Oui, c'est génial! J'ai hâte de commencer. # speaker: Reine

* [Dire au revoir]
    -> au_revoir

=== au_revoir ===
# speaker: Alien
Bon, je dois y aller. À bientôt!

Les portes s'ouvrent. # speaker: Scientifique # action: open_doors_telescope 

-> END