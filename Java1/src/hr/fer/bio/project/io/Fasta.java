package hr.fer.bio.project.io;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileReader;
import java.io.IOException;

/**
 * Created by Branimir on 13.11.2014..
 *
 * @author bpervan
 * @since 1.0
 */
public class Fasta {
    private String comment;
    private String readings;

    private Fasta(String comment, String readings){
        this.comment = comment;
        this.readings = readings;
    }

    public String getComment(){
        return this.comment;
    }

    public String getReadings(){
        return this.readings;
    }

    /**
     * Creates Fasta object from FASTA file given by fileName path
     *
     * @author Branimir
     * @param fileName Path to FASTA file
     * @throws java.io.IOException
     * @return Fasta object representing read FASTA file
     * */
    public static Fasta fromFile(String fileName) throws IOException {
        BufferedReader bufferedReader = new BufferedReader(new FileReader(new File(fileName)));
        StringBuilder stringBuilder = new StringBuilder();

        String comment = bufferedReader.readLine();
        if(!comment.startsWith(">")){
            throw new IOException("Your FASTA file doesn't start with >");
        }
        comment = comment.substring(1);

        String line = null;
        while((line = bufferedReader.readLine()) != null){
            stringBuilder.append(line);
        }
        bufferedReader.close();

        return new Fasta(comment, stringBuilder.toString());
    }
}
