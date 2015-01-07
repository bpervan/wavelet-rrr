package hr.fer.bio.project.tests;

import hr.fer.bio.project.io.Fasta;
import hr.fer.bio.project.rrr.RRRBlock;
import hr.fer.bio.project.wavelet.TreeNode;
import hr.fer.bio.project.wavelet.WaveletTreeBuilder;
import org.junit.*;

import static org.junit.Assert.*;
import static org.junit.Assert.assertEquals;

public class WaveletTreeRRRTest {
    private static Fasta inputFile;
    private static TreeNode<RRRBlock> waveletTree;

    @Before
    public void setUp() throws Exception {
    }

    @After
    public void tearDown() throws Exception {
    }

    @BeforeClass
    public static void setUpClass() throws Exception {
        inputFile = Fasta.fromFile("d:\\ecoli.fa");
        waveletTree = WaveletTreeBuilder.getInstance().fromStringRRR(inputFile.getReadings());
    }

    @AfterClass
    public static void tearDownClass() throws Exception {
        inputFile = null;
        waveletTree = null;
    }

    @Test
    public void GenericTestCase1() throws Exception {
        assertEquals(8670, waveletTree.rank('T', 36000, waveletTree));
    }

    @Test
    public void GenericTestCase2() throws Exception {
        assertEquals(159158, waveletTree.rank('A', 654321, waveletTree));
    }

    @Test
    public void GenericTestCase3() throws Exception {
        assertEquals(7, waveletTree.rank('C', 32, waveletTree));
    }

    @Test
    public void GenericTestCase4() throws Exception {
        assertEquals(133634, waveletTree.rank('A', 548123, waveletTree));
    }

    @Test
    public void GenericTestCase5() throws Exception {
        assertEquals(14532, waveletTree.rank('G', 56148, waveletTree));
    }

    @Test
    public void GenericTestCase6() throws Exception {
        assertEquals(78556, waveletTree.rank('A', 321546, waveletTree));
    }

    @Test
    public void GenericTestCase7() throws Exception {
        assertEquals(19499, waveletTree.rank('T', 75124, waveletTree));
    }

    @Test
    public void GenericTestCase8() throws Exception {
        assertEquals(113274, waveletTree.rank('C', 458741, waveletTree));
    }

    @Test
    public void GenericTestCase9() throws Exception {
        assertEquals(0, waveletTree.rank('G', 2, waveletTree));
    }

    @Test
    public void GenericTestCase10() throws Exception {
        assertEquals(48, waveletTree.rank('T', 158, waveletTree));
    }

    @Test
    public void CharNotInAlphabetTest() throws Exception {
        assertEquals(0, waveletTree.rank('B', 123, waveletTree));
    }

    @Test(expected = IllegalArgumentException.class)
    public void NegativeRankRequestTest() throws Exception {
        waveletTree.rank('A', -1, waveletTree);
    }

    @Test(expected = IllegalArgumentException.class)
    public void RankRequestOverflowTest() throws Exception {
        waveletTree.rank('A', Integer.MAX_VALUE, waveletTree);
    }
}