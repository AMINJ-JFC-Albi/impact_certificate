-> main

=== main ===
#audioDialogue:Chambre/Beaulieu/PeurQueBouche.mp3
#audioRéponse1:Chambre/Joueur/WithBeaulieu-1.mp3
Oh, vous êtes là ! Je commençais à m'inquiéter, ma perfusion s'est arrêtée il y a un moment et j’avais peur que cela se bouche.
    + [Ne vous inquiétez pas pour cela, votre prochaine perfusion est programmée pour 8h.]
        -> TEXT2
-> END

=== TEXT2 ===
#audioDialogue:Chambre/Beaulieu/Daccord.mp3
#audioRéponse1:Chambre/Joueur/WithBeaulieu-2.mp3
D'accord
    + [Je suis d'ailleurs venu vous demander si vous seriez disponible et d’accord ?]
        -> TEXT3
-> END

=== TEXT3 ===
#audioDialogue:Chambre/Beaulieu/Oui.mp3
#audioRéponse1:Chambre/Joueur/WithBeaulieu-3.mp3
Oui, bien sûr.
    + [Parfait, je reviendrai donc avec une nouvelle perfusion.]
        -> TEXT4
-> END

=== TEXT4 ===
#audioDialogue:Chambre/Beaulieu/UneCompote.mp3
#audioRéponse1:Chambre/Joueur/WithBeaulieu-4.mp3
Ah, et sinon, j'ai un peu faim, serait-il possible d'avoir une compote ?
    + [Je vais voir pour que l'on vous en apporte une.]
        #audioDialogue:Chambre/Beaulieu/Merci.mp3
        Très bien, merci.
-> END
        
    
