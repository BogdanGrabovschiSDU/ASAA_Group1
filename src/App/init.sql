--
-- PostgreSQL database dump
--

-- Dumped from database version 15.9 (Debian 15.9-1.pgdg120+1)
-- Dumped by pg_dump version 15.9 (Debian 15.9-1.pgdg120+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: Bike_Parts; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Bike_Parts" (
    "PartID" integer NOT NULL,
    "PartName" character varying(255) NOT NULL,
    "BikeTypeID" integer NOT NULL,
    "Stock" integer NOT NULL
);


ALTER TABLE public."Bike_Parts" OWNER TO postgres;

--
-- Name: Bike_Parts_PartID_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Bike_Parts_PartID_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public."Bike_Parts_PartID_seq" OWNER TO postgres;

--
-- Name: Bike_Parts_PartID_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Bike_Parts_PartID_seq" OWNED BY public."Bike_Parts"."PartID";


--
-- Name: Bike_Parts_Stock; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Bike_Parts_Stock" (
    "StockID" integer NOT NULL,
    "BikeTypeID" integer NOT NULL,
    "PartID" integer NOT NULL,
    "Stock" integer NOT NULL
);


ALTER TABLE public."Bike_Parts_Stock" OWNER TO postgres;

--
-- Name: Bike_Parts_Stock_StockID_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Bike_Parts_Stock_StockID_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public."Bike_Parts_Stock_StockID_seq" OWNER TO postgres;

--
-- Name: Bike_Parts_Stock_StockID_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Bike_Parts_Stock_StockID_seq" OWNED BY public."Bike_Parts_Stock"."StockID";


--
-- Name: Bike_Types; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Bike_Types" (
    "BikeTypeID" integer NOT NULL,
    "BikeType" character varying(255) NOT NULL
);


ALTER TABLE public."Bike_Types" OWNER TO postgres;

--
-- Name: Bike_Types_BikeTypeID_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Bike_Types_BikeTypeID_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public."Bike_Types_BikeTypeID_seq" OWNER TO postgres;

--
-- Name: Bike_Types_BikeTypeID_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Bike_Types_BikeTypeID_seq" OWNED BY public."Bike_Types"."BikeTypeID";


--
-- Name: Bike_Parts PartID; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Bike_Parts" ALTER COLUMN "PartID" SET DEFAULT nextval('public."Bike_Parts_PartID_seq"'::regclass);


--
-- Name: Bike_Parts_Stock StockID; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Bike_Parts_Stock" ALTER COLUMN "StockID" SET DEFAULT nextval('public."Bike_Parts_Stock_StockID_seq"'::regclass);


--
-- Name: Bike_Types BikeTypeID; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Bike_Types" ALTER COLUMN "BikeTypeID" SET DEFAULT nextval('public."Bike_Types_BikeTypeID_seq"'::regclass);


--
-- Data for Name: Bike_Parts; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Bike_Parts" ("PartID", "PartName", "BikeTypeID", "Stock") FROM stdin;
1	Brake	1	50
2	Handlebar	1	20
3	Tire	1	100
4	Brake	2	40
5	Tire	2	80
\.


--
-- Data for Name: Bike_Parts_Stock; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Bike_Parts_Stock" ("StockID", "BikeTypeID", "PartID", "Stock") FROM stdin;
\.


--
-- Data for Name: Bike_Types; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public."Bike_Types" ("BikeTypeID", "BikeType") FROM stdin;
1	Mountain Bike
2	Road Bike
3	Hybrid Bike
\.


--
-- Name: Bike_Parts_PartID_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Bike_Parts_PartID_seq"', 5, true);


--
-- Name: Bike_Parts_Stock_StockID_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Bike_Parts_Stock_StockID_seq"', 1, false);


--
-- Name: Bike_Types_BikeTypeID_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public."Bike_Types_BikeTypeID_seq"', 3, true);


--
-- Name: Bike_Parts_Stock Bike_Parts_Stock_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Bike_Parts_Stock"
    ADD CONSTRAINT "Bike_Parts_Stock_pkey" PRIMARY KEY ("StockID");


--
-- Name: Bike_Parts Bike_Parts_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Bike_Parts"
    ADD CONSTRAINT "Bike_Parts_pkey" PRIMARY KEY ("PartID");


--
-- Name: Bike_Types Bike_Types_BikeType_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Bike_Types"
    ADD CONSTRAINT "Bike_Types_BikeType_key" UNIQUE ("BikeType");


--
-- Name: Bike_Types Bike_Types_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Bike_Types"
    ADD CONSTRAINT "Bike_Types_pkey" PRIMARY KEY ("BikeTypeID");


--
-- Name: Bike_Parts Bike_Parts_BikeTypeID_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Bike_Parts"
    ADD CONSTRAINT "Bike_Parts_BikeTypeID_fkey" FOREIGN KEY ("BikeTypeID") REFERENCES public."Bike_Types"("BikeTypeID");


--
-- Name: Bike_Parts_Stock Bike_Parts_Stock_BikeTypeID_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Bike_Parts_Stock"
    ADD CONSTRAINT "Bike_Parts_Stock_BikeTypeID_fkey" FOREIGN KEY ("BikeTypeID") REFERENCES public."Bike_Types"("BikeTypeID");


--
-- Name: Bike_Parts_Stock Bike_Parts_Stock_PartID_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Bike_Parts_Stock"
    ADD CONSTRAINT "Bike_Parts_Stock_PartID_fkey" FOREIGN KEY ("PartID") REFERENCES public."Bike_Parts"("PartID");


--
-- PostgreSQL database dump complete
--

