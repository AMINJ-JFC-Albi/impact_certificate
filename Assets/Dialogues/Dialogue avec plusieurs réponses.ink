VAR error = false

-> main

=== main ===
#audioDialogue:Infirmerie/IDE/Question.mp3
#audioRéponse1:Infirmerie/Joueur/Question-Reponse1.mp3
#audioRéponse2:Infirmerie/Joueur/Question-Reponse2.mp3
#audioRéponse3:Infirmerie/Joueur/Question-Reponse3.mp3

As-tu une question ?
    + [Puis-je décaler la pose de la perfusion un peu plus tard pour faire la toilette avant ?]
        #audioDialogue:Infirmerie/IDE/Question-Reponse1.mp3
        Oui, bien sûr pas de souci !
        -> END
    + [La prescription a-t-elle été modifiée depuis hier ?]
        #audioDialogue:Infirmerie/IDE/Question-Reponse2-1.mp3
        Tu devrais aller reconsulter la prescription médicale pour en être certain.
        #audioDialogue:Infirmerie/IDE/Question-Reponse2-2.mp3
        Rappelle-toi : Une transmission orale n'est pas une preuve suffisante !
        ~ error = true
        -> END
    + [À quelle heure la perfusion s’est-elle terminée ?]
        #audioDialogue:Infirmerie/IDE/Question-Reponse3.mp3
        Elle s'est terminée il y a peu de temps, tu devrais aller voir le patient avant de préparer sa prochaine perfusion.
        -> END
-> END