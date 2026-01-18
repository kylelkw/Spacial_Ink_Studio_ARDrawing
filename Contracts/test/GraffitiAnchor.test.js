const { expect } = require("chai");
const { ethers } = require("hardhat");

describe("GraffitiAnchor", function () {
  let graffitiAnchor;
  let owner;
  let artist1;
  let artist2;

  beforeEach(async function () {
    [owner, artist1, artist2] = await ethers.getSigners();
    
    const GraffitiAnchor = await ethers.getContractFactory("GraffitiAnchor");
    graffitiAnchor = await GraffitiAnchor.deploy();
    await graffitiAnchor.waitForDeployment();
  });

  describe("Deployment", function () {
    it("Should set the right owner", async function () {
      expect(await graffitiAnchor.owner()).to.equal(owner.address);
    });

    it("Should initialize nextClaimId to 1", async function () {
      expect(await graffitiAnchor.nextClaimId()).to.equal(1);
    });
  });

  describe("Claiming Locations", function () {
    it("Should claim a location successfully", async function () {
      const artistId = "TestArtist";
      const x = 1500000; // 1.5 meters
      const y = 500000;  // 0.5 meters
      const z = 2000000; // 2.0 meters
      const metadataURI = "ipfs://QmTest123";

      const tx = await graffitiAnchor.connect(artist1).claimLocation(
        artistId, x, y, z, metadataURI
      );

      await expect(tx)
        .to.emit(graffitiAnchor, "GraffitiClaimed")
        .withArgs(1, artist1.address, artistId, x, y, z, metadataURI);

      const claim = await graffitiAnchor.getClaim(1);
      expect(claim.artist).to.equal(artist1.address);
      expect(claim.artistId).to.equal(artistId);
      expect(claim.x).to.equal(x);
      expect(claim.y).to.equal(y);
      expect(claim.z).to.equal(z);
      expect(claim.metadataURI).to.equal(metadataURI);
      expect(claim.isActive).to.be.true;
    });

    it("Should not claim the same location twice", async function () {
      const artistId = "TestArtist";
      const x = 1500000;
      const y = 500000;
      const z = 2000000;
      const metadataURI = "ipfs://QmTest123";

      await graffitiAnchor.connect(artist1).claimLocation(
        artistId, x, y, z, metadataURI
      );

      await expect(
        graffitiAnchor.connect(artist2).claimLocation(
          "Artist2", x, y, z, "ipfs://QmTest456"
        )
      ).to.be.revertedWith("Location already claimed");
    });

    it("Should allow claiming after revocation", async function () {
      const artistId = "TestArtist";
      const x = 1500000;
      const y = 500000;
      const z = 2000000;
      const metadataURI = "ipfs://QmTest123";

      await graffitiAnchor.connect(artist1).claimLocation(
        artistId, x, y, z, metadataURI
      );

      await graffitiAnchor.connect(artist1).revokeClaim(1);

      await expect(
        graffitiAnchor.connect(artist2).claimLocation(
          "Artist2", x, y, z, "ipfs://QmTest456"
        )
      ).to.not.be.reverted;
    });
  });

  describe("Updating Metadata", function () {
    it("Should update metadata successfully", async function () {
      const artistId = "TestArtist";
      const x = 1500000;
      const y = 500000;
      const z = 2000000;
      const metadataURI = "ipfs://QmTest123";

      await graffitiAnchor.connect(artist1).claimLocation(
        artistId, x, y, z, metadataURI
      );

      const newURI = "ipfs://QmTest456";
      const tx = await graffitiAnchor.connect(artist1).updateMetadata(1, newURI);

      await expect(tx)
        .to.emit(graffitiAnchor, "MetadataUpdated")
        .withArgs(1, newURI);

      const claim = await graffitiAnchor.getClaim(1);
      expect(claim.metadataURI).to.equal(newURI);
    });

    it("Should not allow non-artist to update metadata", async function () {
      const artistId = "TestArtist";
      const x = 1500000;
      const y = 500000;
      const z = 2000000;
      const metadataURI = "ipfs://QmTest123";

      await graffitiAnchor.connect(artist1).claimLocation(
        artistId, x, y, z, metadataURI
      );

      await expect(
        graffitiAnchor.connect(artist2).updateMetadata(1, "ipfs://QmTest456")
      ).to.be.revertedWith("Only artist can modify their claim");
    });
  });

  describe("Revoking Claims", function () {
    it("Should revoke a claim successfully", async function () {
      const artistId = "TestArtist";
      const x = 1500000;
      const y = 500000;
      const z = 2000000;
      const metadataURI = "ipfs://QmTest123";

      await graffitiAnchor.connect(artist1).claimLocation(
        artistId, x, y, z, metadataURI
      );

      const tx = await graffitiAnchor.connect(artist1).revokeClaim(1);

      await expect(tx)
        .to.emit(graffitiAnchor, "ClaimRevoked")
        .withArgs(1);

      const claim = await graffitiAnchor.getClaim(1);
      expect(claim.isActive).to.be.false;
    });

    it("Should not allow non-artist to revoke claim", async function () {
      const artistId = "TestArtist";
      const x = 1500000;
      const y = 500000;
      const z = 2000000;
      const metadataURI = "ipfs://QmTest123";

      await graffitiAnchor.connect(artist1).claimLocation(
        artistId, x, y, z, metadataURI
      );

      await expect(
        graffitiAnchor.connect(artist2).revokeClaim(1)
      ).to.be.revertedWith("Only artist can modify their claim");
    });
  });

  describe("Query Functions", function () {
    it("Should get artist claims", async function () {
      await graffitiAnchor.connect(artist1).claimLocation(
        "Artist1", 1000000, 1000000, 1000000, "ipfs://1"
      );
      await graffitiAnchor.connect(artist1).claimLocation(
        "Artist1", 2000000, 2000000, 2000000, "ipfs://2"
      );
      await graffitiAnchor.connect(artist2).claimLocation(
        "Artist2", 3000000, 3000000, 3000000, "ipfs://3"
      );

      const artist1Claims = await graffitiAnchor.getArtistClaims(artist1.address);
      expect(artist1Claims.length).to.equal(2);
      expect(artist1Claims[0]).to.equal(1);
      expect(artist1Claims[1]).to.equal(2);

      const artist2Claims = await graffitiAnchor.getArtistClaims(artist2.address);
      expect(artist2Claims.length).to.equal(1);
      expect(artist2Claims[0]).to.equal(3);
    });

    it("Should check location availability", async function () {
      const x = 1500000;
      const y = 500000;
      const z = 2000000;

      expect(await graffitiAnchor.isLocationAvailable(x, y, z)).to.be.true;

      await graffitiAnchor.connect(artist1).claimLocation(
        "Artist1", x, y, z, "ipfs://1"
      );

      expect(await graffitiAnchor.isLocationAvailable(x, y, z)).to.be.false;

      await graffitiAnchor.connect(artist1).revokeClaim(1);

      expect(await graffitiAnchor.isLocationAvailable(x, y, z)).to.be.true;
    });
  });
});
